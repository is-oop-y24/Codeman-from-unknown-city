using System;

namespace Shops.Interfaces
{
    public interface IProductsNames
    {
        Guid Register(string name);
        string GetName(Guid id);
        bool CheckExistence(Guid id);
    }
}