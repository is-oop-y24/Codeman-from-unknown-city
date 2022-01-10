using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Backups
{
    public class RepositoryWithSplitStoragesAlgorithm : Repository
    {
        public RepositoryWithSplitStoragesAlgorithm(string path, IStorageFactory storageFactory, bool fsIsVirtual = false)
            : base(path, storageFactory, fsIsVirtual)
        { }

        public override IEnumerable<IStorage> Save(IJobObject jobObjects)
        {
            var storages = new List<IStorage>();
            GenerateNewPrefix();
            foreach (string srcPath in jobObjects.FilesPaths)
            {
                string srcName = Path.GetFileName(srcPath);
                string copyPath = BuildCopyPath(srcName);
                if (!FsIsVirtual)
                {
                    ZipArchive archive = ZipFile.Open(copyPath, ZipArchiveMode.Create);
                    archive.CreateEntryFromFile(srcPath, srcName);
                }

                storages.Add(StorageFactory.Create(copyPath, FsIsVirtual));
            }

            return storages;
        }
    }
}