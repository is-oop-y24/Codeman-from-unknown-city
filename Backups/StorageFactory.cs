namespace Backups
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage Create(string path)
        {
            return new Storage(path);
        }
    }
}