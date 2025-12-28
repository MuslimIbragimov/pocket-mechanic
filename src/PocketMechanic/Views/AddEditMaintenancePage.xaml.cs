using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Views
{
    public partial class AddEditMaintenancePage : ContentPage
    {
        public AddEditMaintenancePage(AddEditMaintenanceViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
