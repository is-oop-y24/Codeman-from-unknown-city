using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Backups
{
    public class RepositoryWithSingleStorageAlgorithm : Repository
    {
        public const string DefaultCopyName = "backup";
        private readonly string _copyName;

        public RepositoryWithSingleStorageAlgorithm(string path, IStorageFactory storageFactory, string copyName = DefaultCopyName)
            : base(path, storageFactory)
        {
            if (string.IsNullOrEmpty(copyName))
                throw new ArgumentException(@"{nameof(copyName)} shouldn't be null or empty");
            _copyName = copyName;
        }

        public override IEnumerable<IStorage> Save(IJobObject jobObject)
        {
            var storages = new List<IStorage>();
            GenerateNewPrefix();
            string copyPath = BuildCopyPath(_copyName);
            ZipArchive archive = ZipFile.Open(copyPath, ZipArchiveMode.Create);
            foreach (string srcPath in jobObject.FilesPaths)
            {
                archive.CreateEntryFromFile(srcPath, Path.GetFileName(srcPath));
            }

            storages.Add(StorageFactory.Create(copyPath));
            return storages;
        }
    }
}