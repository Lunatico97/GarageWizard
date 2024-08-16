using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GarageCoreMVC.Data
{
    [ExcludeFromCodeCoverage]
    public class Storage<T> : IStorage<T> where T : class
    {
        private string? _path = null;
        private Stream? _gstream = null;
        private XmlSerializer _serde;

        public Storage()
        {
            _serde = new XmlSerializer(typeof(List<T>));
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public List<T>? LoadBackup()
        {
            List<T>? vehicles = new List<T>();
            if (_path != null)
            {
                _gstream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Read);
                vehicles = _serde.Deserialize(_gstream) as List<T>;
                _gstream.Close();
            }
            else Console.WriteLine("File path is null !!!");
            return vehicles;
        }

        public void SaveBackup(List<T> objects)
        {
            if (_path != null)
            {
                File.WriteAllText(_path, string.Empty);
                _gstream = new FileStream(_path, FileMode.Open, FileAccess.Write);
                _serde.Serialize(_gstream, objects as List<T>);
                _gstream.Close();
            }
            else Console.WriteLine("File path is null !!!");
        }
    }
}
