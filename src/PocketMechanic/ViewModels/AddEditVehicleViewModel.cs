using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PocketMechanic.Models;
using PocketMechanic.Services;
using System;
using System.Threading.Tasks;

namespace PocketMechanic.ViewModels
{
    [QueryProperty(nameof(VehicleId), "vehicleId")]
    public partial class AddEditVehicleViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _vehicleId;

        [ObservableProperty]
        private string _make;

        [ObservableProperty]
        private string _model;

        [ObservableProperty]
        private int _year = DateTime.Now.Year;

        [ObservableProperty]
        private string _vin;

        [ObservableProperty]
        private string _licensePlate;

        [ObservableProperty]
        private int _currentMileage;

        [ObservableProperty]
        private DateTime? _purchaseDate;

        [ObservableProperty]
        private string _nickname;

        [ObservableProperty]
        private string _pageTitle = "Add Vehicle";

        [ObservableProperty]
        private bool _isEditMode;

        public AddEditVehicleViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnVehicleIdChanged(int value)
        {
            if (value > 0)
            {
                IsEditMode = true;
                PageTitle = "Edit Vehicle";
                Task.Run(async () => await LoadVehicleAsync());
            }
        }

        private async Task LoadVehicleAsync()
        {
            var vehicle = await _databaseService.GetVehicleAsync(VehicleId);
            if (vehicle != null)
            {
                Make = vehicle.Make;
                Model = vehicle.Model;
                Year = vehicle.Year;
                Vin = vehicle.VIN;
                LicensePlate = vehicle.LicensePlate;
                CurrentMileage = vehicle.CurrentMileage;
                PurchaseDate = vehicle.PurchaseDate;
                Nickname = vehicle.Nickname;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Make) || string.IsNullOrWhiteSpace(Model))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter Make and Model", "OK");
                return;
            }

            var vehicle = new Vehicle
            {
                Id = VehicleId,
                Make = Make,
                Model = Model,
                Year = Year,
                VIN = Vin,
                LicensePlate = LicensePlate,
                CurrentMileage = CurrentMileage,
                PurchaseDate = PurchaseDate,
                Nickname = Nickname
            };

            await _databaseService.SaveVehicleAsync(vehicle);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
