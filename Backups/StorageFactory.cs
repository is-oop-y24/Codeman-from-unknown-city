namespace Backups
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage Create(string path, bool fsIsVirtual)
        {
            return new Storage(path, fsIsVirtual);
        }
    }
}