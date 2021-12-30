using System;
using System.Collections.Generic;

namespace Backups
{
    public interface IRepository
    {
        public IEnumerable<IStorage> Save(IJobObject jobObject);
    }
}