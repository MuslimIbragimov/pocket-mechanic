using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace PocketMechanic
{
    public static class MauiProgram
    {
        public static IHostBuilder CreateMauiApp() =>
            Host.CreateDefaultBuilder()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureServices(services =>
                {
                    // Register application services here
                })
                .ConfigureMauiHandlers(handlers =>
                {
                    // Configure custom handlers here
                })
                .UseMauiApp<App>();
    }
}