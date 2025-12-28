using Microsoft.Maui.Controls;
using PocketMechanic.Views;

namespace PocketMechanic
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("addvehicle", typeof(AddEditVehiclePage));
            Routing.RegisterRoute("vehicledetail", typeof(VehicleDetailPage));
            Routing.RegisterRoute("addmaintenance", typeof(AddEditMaintenancePage));
        }
    }
}
