using Shops.Interfaces;

namespace Shops.Entities
{
    public class ShopFactory : IShopFactory
    {
        private IProductsNames _productsNames;

        public ShopFactory(IProductsNames productsNames)
        {
            _productsNames = productsNames;
        }

        public IShop CreateShop(string name, string address) => new Shop(_productsNames, name, address);
    }
}