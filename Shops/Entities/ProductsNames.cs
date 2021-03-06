using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Interfaces;

namespace Shops.Entities
{
    public class ProductsNames : IProductsNames
    {
        private readonly Dictionary<Guid, string> _productsNames = new ();

        public Guid Register(string name)
        {
            if (_productsNames.ContainsValue(name))
            {
                return _productsNames.First(idNamePair => idNamePair.Value == name).Key;
            }

            var id = Guid.NewGuid();
            _productsNames[id] = name;
            return id;
        }

        public string GetName(Guid id) => _productsNames[id];

        public bool CheckExistence(Guid id) => _productsNames.ContainsKey(id);
    }
}