namespace Shops.Interfaces
{
    public interface IShopFactory
    {
        public IShop CreateShop(string name, string address);
    }
}