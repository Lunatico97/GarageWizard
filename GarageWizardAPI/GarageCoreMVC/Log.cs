using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC
{
    public interface ILog
    {
        void showLog(string message);
    }

    [ExcludeFromCodeCoverage]
   public class Logger : ILog
    {
        public void showLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
