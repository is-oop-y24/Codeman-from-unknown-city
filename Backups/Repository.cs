using System;
using System.Collections.Generic;
using System.IO;

namespace Backups
{
    public abstract class Repository : IRepository
    {
        private readonly string _selfPath;
        private uint _prefix;

        protected Repository(string path, IStorageFactory storageFactory)
        {
            StorageFactory = storageFactory ?? throw new ArgumentNullException(nameof(storageFactory));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _selfPath = path;
            _prefix = 0;
        }

        protected IStorageFactory StorageFactory { get; }

        public abstract IEnumerable<IStorage> Save(IJobObject jobObject);

        protected void GenerateNewPrefix() => ++_prefix;
        protected string BuildCopyPath(string srcName) => Path.Combine(_selfPath, $@"{srcName}_{_prefix}.zip");
    }
}