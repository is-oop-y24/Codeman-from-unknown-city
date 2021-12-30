using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backups.FileSystemTest
{
    public static class FsTests
    {
        private const string TestFilesDirPath = "/home/sergei/RiderProjects/Codeman-from-unknown-city/Backups";

        public static void TestSingleStorageBackup()
        {
            const string repositoryName = ".test_backup_single";
            IBackupJob backupJob = new BackupJob(
                new JobObject(),
                new RepositoryWithSingleStorageAlgorithm(repositoryName, new StorageFactory()));
            var paths = new List<string>
            {
                Path.Combine(TestFilesDirPath, "Program.cs"),
                Path.Combine(TestFilesDirPath, "Storage.cs"),
                Path.Combine(TestFilesDirPath, "RestorePoint.cs"),
            };
            paths.ForEach(action: path =>
            {
                if (!backupJob.Add(path))
                    throw new ApplicationException($"Can't add '{path}' to the backup job. (The file doesn't exists)");
            });
            backupJob.Run();
            backupJob.Run();
            if (Directory
                .GetFiles(repositoryName, $"{RepositoryWithSingleStorageAlgorithm.DefaultCopyName}*.zip")
                .Length != 2)
            {
                Directory.Delete(repositoryName, true);
                throw new ApplicationException("Backup files weren't created");
            }

            Directory.Delete(repositoryName, true);
        }

        public static void TestSplitStorageBackup()
        {
            const string repositoryName = ".test_backup_split";
            const int nCopiesOfFirst = 1;
            const int nCopiesOfLast = 2;
            IBackupJob backupJob = new BackupJob(
                new JobObject(),
                new RepositoryWithSplitStoragesAlgorithm(repositoryName, new StorageFactory()));
            var testFilesNames = new List<string> { "Program", "Storage" };
            var paths = testFilesNames.Select(name => Path.Combine(TestFilesDirPath, $"{name}.cs")).ToList();
            paths.ForEach(action: path =>
            {
                if (!backupJob.Add(path))
                    throw new ApplicationException($"Can't add '{path}' to the backup job. (The file doesn't exists)");
            });
            backupJob.Run();
            backupJob.Remove(paths.First());
            backupJob.Run();
            string[] copiesOfFirst = Directory.GetFiles(repositoryName, $"{testFilesNames.First()}*.zip");
            string[] copiesOfLast = Directory.GetFiles(repositoryName, $"{testFilesNames.Last()}*.zip");
            if (copiesOfFirst.Length != nCopiesOfFirst || copiesOfLast.Length != nCopiesOfLast)
            {
                Directory.Delete(repositoryName, true);
                throw new ApplicationException("Backup files weren't created");
            }

            Directory.Delete(repositoryName, true);
        }
    }
}