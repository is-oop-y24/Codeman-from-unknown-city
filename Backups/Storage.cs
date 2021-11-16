using System;
using System.IO;

namespace Backups
{
    public class Storage : IStorage
    {
        public Storage(string pathToCopy, bool fsIsVirtual)
        {
            if (string.IsNullOrEmpty(pathToCopy))
                throw new ArgumentException("Path to copy should not be null or empty");
            if (!fsIsVirtual && !File.Exists(pathToCopy))
                throw new ArgumentException($@"{pathToCopy} doesn't exists");
            CopyInfo = pathToCopy;
        }

        public string CopyInfo { get; }
    }
}