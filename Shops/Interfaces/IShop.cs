using System;
using System.Collections.Generic;

namespace Shops.Interfaces
{
    public interface IShop
    {
        public string Name { get; }
        public string Address { get; }
        IShop AddProductsInAssortment(List<(Guid productId, float productCost)> productsCosts);
        IShop TakeDelivery(List<(Guid productId, uint productCount)> delivery);
        IShop SellProducts(List<(Guid productId, uint produtCount)> order, ref float money);
        IShop ChangeCost(Guid id, float cost);
        bool Contains(Guid id);
        float GetCost(Guid id);
        uint GetCount(Guid id);
    }
}