using GarageCoreMVC;
using Microsoft.AspNetCore;
using Serilog.Extensions.Logging.File;
using System.Diagnostics.CodeAnalysis;

// Making a class just for entry point so that configurations and routing can be maintained in other classes !
namespace GarageCoreMVC {

    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            // Startup is a class that abstracts the operation of a web builder application !
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    logging => {
                        logging.ClearProviders();
                        logging.AddFile($"Logs/Log.txt");
                        logging.AddConsole();
                        logging.AddDebug();
                    })
                .UseStartup<GStartup>()
                .Build();
        }
    }

}
