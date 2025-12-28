using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Views
{
    public partial class VehicleDetailPage : ContentPage
    {
        public VehicleDetailPage(VehicleDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
