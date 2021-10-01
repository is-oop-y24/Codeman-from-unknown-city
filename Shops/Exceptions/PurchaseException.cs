using System;

namespace Shops.Exceptions
{
    public class PurchaseException : Exception
    {
        public PurchaseException(string what)
            : base($"Can't buy a product: {what}")
        {
        }
    }
}
