using System.Collections.Generic;

namespace Backups
{
    public interface IBackupJob
    {
        public IEnumerable<RestorePoint> Backup { get; }
        public bool Add(string path);
        public bool Remove(string path);
        void Run();
    }
}