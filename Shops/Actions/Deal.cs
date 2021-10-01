using System.Collections.Generic;
using Shops.Entities;

namespace Shops.Actions
{
    public class Deal
    {
        public static void Resolve(Shop shop, ref Customer customer, List<(string, uint)> productsNamesAndCounts) => shop.SellProducts(productsNamesAndCounts, ref customer.Money);
    }
}
