using System;
using System.Collections.Generic;
using Shops.Interfaces;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop : IShop
    {
        private readonly Dictionary<Guid, ProductInfo> _productsInfo;
        private readonly IProductsNames _productsNames;

        public Shop(IProductsNames productsNames, string name, string address)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address))
            {
                throw new ShopsException("Shop name and address shouldn't be empty or null");
            }

            Name = name;
            Address = address;
            _productsInfo = new Dictionary<Guid, ProductInfo>();
            _productsNames = productsNames ??
                                       throw new ShopsException("Products names repository shouldn't be null");
        }

        private Shop(Shop other)
            : this(other._productsNames, other.Name, other.Address)
        {
            foreach ((Guid id, ProductInfo productInfo) in other._productsInfo)
            {
                _productsInfo[id] = productInfo;
            }
        }

        private Shop(Shop other, List<(Guid, uint)> updatedProductsCounts)
            : this(other)
        {
            foreach ((Guid id, uint count) in updatedProductsCounts)
            {
                CheckExistence(id);
                _productsInfo[id] = new ProductInfo(_productsInfo[id].Cost, count);
            }
        }

        private Shop(Shop other, List<(Guid, float)> updatedProductsCosts)
            : this(other)
        {
            foreach ((Guid id, float cost) in updatedProductsCosts)
            {
                if (!_productsNames.CheckExistence(id))
                {
                    throw new ShopsException($"Product with id {id} is not exists");
                }

                _productsInfo[id] = _productsInfo.ContainsKey(id)
                    ? new ProductInfo(cost, _productsInfo[id].Count)
                    : new ProductInfo(cost);
            }
        }

        public string Name { get; }
        public string Address { get; }

        public IShop AddProductsInAssortment(List<(Guid, float)> productsCost) => new Shop(this, productsCost);

        public IShop TakeDelivery(List<(Guid, uint)> delivery) => new Shop(this, delivery);

        public IShop SellProducts(List<(Guid, uint)> order, ref float money)
            => new Shop(this, HandleOrder(order, ref money));

        public IShop ChangeCost(Guid id, float cost) => new Shop(this, new List<(Guid, float)> { (id, cost) });

        public bool Contains(Guid id) => _productsInfo.ContainsKey(id);

        public float GetCost(Guid id) => _productsInfo[id].Cost;
        public uint GetCount(Guid id) => _productsInfo[id].Count;

        private List<(Guid, uint)> HandleOrder(List<(Guid, uint)> order, ref float money)
        {
            float moneyBefore = money;
            string errorMsg;
            var soldProducts = new List<(Guid, uint)>();

            foreach ((Guid id, uint count) in order)
            {
                CheckExistence(id);
                uint nProductsBefore = _productsInfo[id].Count;

                if (nProductsBefore < count)
                {
                    errorMsg = $"Insufficient amount of products ({_productsNames.GetName(id)}";
                    goto Cleanup;
                }

                money -= count * _productsInfo[id].Cost;

                if (money < 0)
                {
                    errorMsg = "Insufficient funds";
                    goto Cleanup;
                }

                soldProducts.Add((id, nProductsBefore - count));
            }

            return soldProducts;

            Cleanup:
            money = moneyBefore;
            throw new ShopsException(errorMsg);
        }

        private void CheckExistence(Guid id)
        {
            if (!_productsNames.CheckExistence(id))
            {
                throw new ShopsException($"Product with id {id} is not exists");
            }

            if (!_productsInfo.ContainsKey(id))
            {
                throw new ShopsException(
                    $"Product '{_productsNames.GetName(id)}' isn't in '{Name}' assortment");
            }
        }

        private class ProductInfo
        {
            public ProductInfo(float cost, uint count = 0)
            {
                if (cost < 1)
                {
                    throw new ShopsException("Product cost should be bigger than 1");
                }

                Cost = cost;
                Count = count;
            }

            public float Cost { get; }
            public uint Count { get; }
        }
    }
}