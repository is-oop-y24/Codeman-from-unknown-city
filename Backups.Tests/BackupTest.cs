using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Backups.Tests
{
    public class BackupTest
    {
        private const string SplitStorageBackupJobName = ".test_backup_split";
        private const string TestFilesDirPath = "some_test_dir_path";
        
        private IBackupJob _splitStorageBackupJob;
        
        [SetUp]
        public void Setup()
        {
            _splitStorageBackupJob = new BackupJob(
                new JobObject(true),
                new RepositoryWithSplitStoragesAlgorithm(SplitStorageBackupJobName, new StorageFactory(), true)
            );
        }

        [Test]
        public void SplitStoragesBackup_CreatedRightNumberOfEntities()
        {
            const int nRestorePoints = 2;
            const int nStorages = 3;
            var paths = new List<string>
            {
                "some_path",
                "some_another_path"
            };
            paths.ForEach(path => Assert.True(_splitStorageBackupJob.Add(path)));
            _splitStorageBackupJob.Run();
            _splitStorageBackupJob.Remove("some_path");
            _splitStorageBackupJob.Run();
            var backup = _splitStorageBackupJob.Backup.ToList();
            Assert.True(backup.Count() == nRestorePoints);
            Assert.True(nStorages == backup.Sum(restorePoint => restorePoint.Storages.Count()));
        }
    }
}