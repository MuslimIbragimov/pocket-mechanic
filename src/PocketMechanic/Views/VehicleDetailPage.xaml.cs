using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;

namespace PocketMechanic.Views
{
    public partial class VehicleDetailPage : ContentPage
    {
        private readonly VehicleDetailViewModel _viewModel;

        public VehicleDetailPage(VehicleDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.RefreshCommand.ExecuteAsync(null);
        }
    }
}
