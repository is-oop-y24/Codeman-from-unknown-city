using System;
using System.Collections.Generic;

namespace Backups
{
    public class BackupJob : IBackupJob
    {
        private readonly List<RestorePoint> _backup;
        private readonly IJobObject _jobObject;
        private readonly IRepository _repository;

        public BackupJob(IJobObject jobObject, IRepository repository)
        {
            _backup = new List<RestorePoint>();
            _jobObject = jobObject ?? throw new ArgumentNullException(nameof(jobObject));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<RestorePoint> Backup => new List<RestorePoint>(_backup);

        public bool Add(string path) => _jobObject.Add(path);

        public bool Remove(string path) => _jobObject.Remove(path);

        public void Run()
        {
            IEnumerable<IStorage> storages = _repository.Save(_jobObject);
            _backup.Add(new RestorePoint(DateTime.Now, storages));
        }
    }
}