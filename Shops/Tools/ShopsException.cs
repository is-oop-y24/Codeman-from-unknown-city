using System;

namespace Shops.Tools
{
    public class ShopsException : Exception
    {
        public ShopsException(string what)
            : base(what)
        { }
    }
}