using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PocketMechanic.Models;
using PocketMechanic.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PocketMechanic.ViewModels
{
    public partial class GarageViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Vehicle> _vehicles;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _hasVehicles;

        public GarageViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _vehicles = new ObservableCollection<Vehicle>();
        }

        [RelayCommand]
        private async Task LoadVehiclesAsync()
        {
            IsRefreshing = true;
            
            var vehicles = await _databaseService.GetVehiclesAsync();
            Vehicles.Clear();
            foreach (var vehicle in vehicles)
            {
                Vehicles.Add(vehicle);
            }

            HasVehicles = Vehicles.Count > 0;
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task AddVehicleAsync()
        {
            await Shell.Current.GoToAsync("addvehicle");
        }

        [RelayCommand]
        private async Task SelectVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                return;

            await Shell.Current.GoToAsync($"vehicledetail?vehicleId={vehicle.Id}");
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadVehiclesAsync();
        }
    }
}
