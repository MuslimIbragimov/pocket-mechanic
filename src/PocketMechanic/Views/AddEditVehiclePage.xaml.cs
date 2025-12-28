using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Views
{
    public partial class AddEditVehiclePage : ContentPage
    {
        public AddEditVehiclePage(AddEditVehicleViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
