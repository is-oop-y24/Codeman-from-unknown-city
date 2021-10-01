using System;
using System.Collections.Generic;
using Shops.Exceptions;

namespace Shops.Entities
{
    public class Shop
    {
        private readonly List<Product> _products;

        public Shop(string name, string address)
        {
            Name = name;
            Address = address;
            Id = Guid.NewGuid();
            _products = new List<Product>();
        }

        public string Name { get; }
        public string Address { get; }
        public Guid Id { get; }

        public void AddProductsInAssortment(List<(string, uint)> productNamesAndCosts)
        {
            foreach ((string name, uint cost) in productNamesAndCosts)
            {
                if (_products.Exists(product => product.Name == name))
                {
                    throw new ShopException($"Product '{name}' already exists");
                }

                _products.Add(new Product(name, cost));
            }
        }

        public void ChangeCost(string productName, uint newCost) => GetProduct(productName).Cost = newCost;

        public void TakeProductsDelivery(List<(string, uint)> namesAndCounts)
        {
            foreach ((string name, uint count) in namesAndCounts)
            {
                GetProduct(name).Count += count;
            }
        }

        public void SellProducts(List<(string, uint)> namesAndCounts, ref uint money)
        {
            var purchasedProducts = new List<(Product, uint)>();
            uint costOfAll = 0;

            foreach ((string name, uint count) in namesAndCounts)
            {
                Product product = GetProduct(name);

                if (product.Count < count)
                {
                    throw new PurchaseException("Insufficient amount");
                }

                costOfAll += product.Cost * count;
                purchasedProducts.Add((product, count));
            }

            if (money < costOfAll)
            {
                throw new PurchaseException("Insufficient funds");
            }

            foreach ((Product productInfo, uint count) in purchasedProducts)
            {
                productInfo.Count -= count;
            }

            money -= costOfAll;
        }

        public uint GetProductCost(string name) => GetProduct(name).Cost;
        public uint GetProductCount(string name) => GetProduct(name).Count;

        private Product GetProduct(string name) => _products.Find(product => product.Name == name) ?? throw new ShopException($"Product {name} doesn't exists");

        private class Product
        {
            public Product(string name, uint cost)
            {
                Name = name;
                Cost = cost;
                Count = 0;
            }

            public string Name { get; }
            public uint Cost { get; set; }
            public uint Count { get; set; }
        }
    }
}
