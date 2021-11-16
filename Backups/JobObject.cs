using System.Collections.Generic;
using System.IO;

namespace Backups
{
    public class JobObject : IJobObject
    {
        private readonly List<string> _filesPaths = new ();

        public List<string> FilesPaths => new (_filesPaths);

        public bool Add(string path)
        {
            bool pathValid = File.Exists(path);
            if (pathValid)
                _filesPaths.Add(path);
            return pathValid;
        }

        public bool Remove(string path) => _filesPaths.Remove(path);
    }
}