using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Reports.Models;

namespace Reports.Repository
{
    public class Repository<T> where T : IHaveId
    {
        private readonly string _path;
        
        public Repository(string path)
        {
            _path = path;
            if (!File.Exists(_path))
                File.Create(_path);
        }
        
        public void Save(IEnumerable<T> entities)
        {
            string json = JsonConvert.SerializeObject(entities);
            File.AppendAllText(_path, json);
        }

        public IEnumerable<T> Load()
        {
            using var streamReader = new StreamReader(_path);
            string json = streamReader.ReadToEnd();
            IEnumerable<T> entities = JsonConvert.DeserializeObject<IEnumerable<T>>(json);

            return entities;
        }
    }
}