using System;
using System.Collections.Generic;

namespace Shops.Interfaces
{
    public interface IShop
    {
        public string Name { get; }
        public string Address { get; }
        IShop AddProductsInAssortment(List<(Guid, float)> productsCost);
        IShop TakeDelivery(List<(Guid, uint)> productsCounts);
        IShop SellProducts(List<(Guid, uint)> order, ref float money);
        IShop ChangeCost(Guid id, float cost);
        bool Contains(Guid id);
        float GetCost(Guid id);
        uint GetCount(Guid id);
    }
}