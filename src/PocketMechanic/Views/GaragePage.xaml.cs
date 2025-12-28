using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Views
{
    public partial class GaragePage : ContentPage
    {
        public GaragePage(GarageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (BindingContext is GarageViewModel viewModel)
            {
                await viewModel.LoadVehiclesCommand.ExecuteAsync(null);
            }
        }
    }
}
