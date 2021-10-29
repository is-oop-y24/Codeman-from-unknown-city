using System;

namespace Shops.Interfaces
{
    public interface IProductsNames
    {
        Guid Register(string name);
        string GetName(Guid id);
        void CheckExistence(Guid id);
    }
}