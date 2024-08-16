
namespace GarageCoreMVC.Data
{
    public interface IStorage<T> where T : class
    {
        void SetPath(string path);
        List<T>? LoadBackup();
        void SaveBackup(List<T> objects);
    }
}
