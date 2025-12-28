using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PocketMechanic.Models;
using PocketMechanic.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PocketMechanic.ViewModels
{
    [QueryProperty(nameof(VehicleId), "vehicleId")]
    public partial class VehicleDetailViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _vehicleId;

        [ObservableProperty]
        private Vehicle _vehicle;

        [ObservableProperty]
        private ObservableCollection<MaintenanceRecord> _maintenanceRecords;

        [ObservableProperty]
        private ObservableCollection<MaintenanceRecord> _overdueItems;

        [ObservableProperty]
        private ObservableCollection<MaintenanceRecord> _dueSoonItems;

        [ObservableProperty]
        private bool _hasOverdueItems;

        [ObservableProperty]
        private bool _hasDueSoonItems;

        [ObservableProperty]
        private bool _hasMaintenanceHistory;

        [ObservableProperty]
        private int _newMileage;

        public VehicleDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _maintenanceRecords = new ObservableCollection<MaintenanceRecord>();
            _overdueItems = new ObservableCollection<MaintenanceRecord>();
            _dueSoonItems = new ObservableCollection<MaintenanceRecord>();
        }

        partial void OnVehicleIdChanged(int value)
        {
            if (value > 0)
            {
                Task.Run(async () => await LoadVehicleAsync());
            }
        }

        [RelayCommand]
        private async Task LoadVehicleAsync()
        {
            Vehicle = await _databaseService.GetVehicleAsync(VehicleId);
            if (Vehicle != null)
            {
                NewMileage = Vehicle.CurrentMileage;
                await LoadMaintenanceRecordsAsync();
            }
        }

        private async Task LoadMaintenanceRecordsAsync()
        {
            var records = await _databaseService.GetMaintenanceRecordsAsync(VehicleId);
            MaintenanceRecords.Clear();
            foreach (var record in records)
            {
                MaintenanceRecords.Add(record);
            }

            var overdue = await _databaseService.GetOverdueMaintenanceAsync(VehicleId, Vehicle.CurrentMileage);
            OverdueItems.Clear();
            foreach (var item in overdue)
            {
                OverdueItems.Add(item);
            }

            var dueSoon = await _databaseService.GetDueSoonMaintenanceAsync(VehicleId, Vehicle.CurrentMileage);
            DueSoonItems.Clear();
            foreach (var item in dueSoon)
            {
                DueSoonItems.Add(item);
            }

            HasOverdueItems = OverdueItems.Count > 0;
            HasDueSoonItems = DueSoonItems.Count > 0;
            HasMaintenanceHistory = MaintenanceRecords.Count > 0;
        }

        [RelayCommand]
        private async Task AddMaintenanceAsync()
        {
            await Shell.Current.GoToAsync($"addmaintenance?vehicleId={VehicleId}");
        }

        [RelayCommand]
        private async Task SelectMaintenanceAsync(MaintenanceRecord record)
        {
            if (record == null)
                return;

            await Shell.Current.GoToAsync($"maintenancedetail?maintenanceId={record.Id}");
        }

        [RelayCommand]
        private async Task EditVehicleAsync()
        {
            await Shell.Current.GoToAsync($"addvehicle?vehicleId={VehicleId}");
        }

        [RelayCommand]
        private async Task UpdateMileageAsync()
        {
            if (NewMileage < Vehicle.CurrentMileage)
            {
                await Shell.Current.DisplayAlert("Error", "New mileage cannot be less than current mileage", "OK");
                return;
            }

            await _databaseService.UpdateVehicleMileageAsync(VehicleId, NewMileage);
            await LoadVehicleAsync();
            await Shell.Current.DisplayAlert("Success", "Mileage updated successfully", "OK");
        }

        [RelayCommand]
        private async Task DeleteVehicleAsync()
        {
            var result = await Shell.Current.DisplayAlert(
                "Delete Vehicle", 
                "Are you sure you want to delete this vehicle? All maintenance records will also be deleted.", 
                "Delete", 
                "Cancel");

            if (result)
            {
                await _databaseService.DeleteVehicleAsync(Vehicle);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadVehicleAsync();
        }
    }
}
