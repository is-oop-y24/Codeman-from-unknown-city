using System;

namespace Banks.UI
{
    public class MenuException : Exception
    {
        public MenuException(string what)
            : base(what)
        { }
    }
}