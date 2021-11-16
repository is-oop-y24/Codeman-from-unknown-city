namespace Backups.FileSystemTest
{
    internal static class Program
    {
        private static void Main()
        {
            FsTests.TestSingleStorageBackup();
            FsTests.TestSplitStorageBackup();
        }
    }
}