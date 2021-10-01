namespace Shops.Entities
{
    public struct Customer
    {
        public readonly string Name;
        public uint Money;

        public Customer(string name, uint money)
        {
            Name = name;
            Money = money;
        }
    }
}
