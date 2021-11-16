namespace Backups
{
    public interface IStorageFactory
    {
        public IStorage Create(string path);
    }
}