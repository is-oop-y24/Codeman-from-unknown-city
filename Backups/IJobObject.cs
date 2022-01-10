using System.Collections.Generic;
using System.IO;

namespace Backups
{
    public interface IJobObject
    {
        public List<string> FilesPaths { get; }
        public bool Add(string path);
        public bool Remove(string path);
    }
}