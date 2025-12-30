using Microsoft.Maui.Controls;
using PocketMechanic.ViewModels;
using PocketMechanic.Models;

namespace PocketMechanic.Views
{
    public partial class AddEditVehiclePage : ContentPage
    {
        private readonly AddEditVehicleViewModel _viewModel;

        public AddEditVehiclePage(AddEditVehicleViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        private void OnMakeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedMake = e.CurrentSelection[0] as MakeItem;
                _viewModel.SelectedMake = selectedMake;
                _viewModel.ShowMakePicker = false;
                
                // Clear selection for next time
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}
