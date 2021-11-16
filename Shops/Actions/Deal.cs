using System;
using System.Collections.Generic;
using Shops.Entities;
using Shops.Interfaces;

namespace Shops.Actions
{
    public static class Deal
    {
        public static (IShop, Customer) Resolve(IShop shop, Customer customer, List<(Guid, uint)> order)
        {
            float money = customer.Money;
            return (shop.SellProducts(order, ref money), new Customer(customer.Name, money));
        }
    }
}
