using System;
using System.Collections.Generic;
using System.IO;

namespace Backups
{
    public abstract class Repository : IRepository
    {
        private readonly string _selfPath;
        private uint _prefix;

        protected Repository(string path, IStorageFactory storageFactory, bool virtualFsIsVirtual)
        {
            FsIsVirtual = virtualFsIsVirtual;
            StorageFactory = storageFactory ?? throw new ArgumentNullException(nameof(storageFactory));
            if (!virtualFsIsVirtual && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            _selfPath = path;
            _prefix = 0;
        }

        protected IStorageFactory StorageFactory { get; }
        protected bool FsIsVirtual { get; }

        public abstract IEnumerable<IStorage> Save(IJobObject jobObject);

        protected void GenerateNewPrefix() => ++_prefix;
        protected string BuildCopyPath(string srcName) => Path.Combine(_selfPath, $@"{srcName}_{_prefix}.zip");
    }
}