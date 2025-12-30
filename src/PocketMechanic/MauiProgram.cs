using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using PocketMechanic.Services;
using PocketMechanic.ViewModels;
using PocketMechanic.Views;

namespace PocketMechanic
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<VehicleInfoService>();

            // Register ViewModels
            builder.Services.AddTransient<GarageViewModel>();
            builder.Services.AddTransient<AddEditVehicleViewModel>();
            builder.Services.AddTransient<VehicleDetailViewModel>();
            builder.Services.AddTransient<AddEditMaintenanceViewModel>();

            // Register Views
            builder.Services.AddTransient<GaragePage>();
            builder.Services.AddTransient<AddEditVehiclePage>();
            builder.Services.AddTransient<VehicleDetailPage>();
            builder.Services.AddTransient<AddEditMaintenancePage>();

            return builder.Build();
        }
    }
}