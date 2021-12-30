using System;
using System.Collections.Generic;
using System.Linq;

namespace Backups
{
    public readonly struct RestorePoint
    {
        public readonly DateTime Date;
        public readonly IEnumerable<IStorage> Storages;

        public RestorePoint(DateTime date, IEnumerable<IStorage> storages)
        {
            Date = date;
            Storages = storages ?? throw new ArgumentNullException(nameof(storages));
            if (Storages.Count() == 0)
                throw new ArgumentException("Storages collection shouldn't be empty");
            Date = date;
        }
    }
}