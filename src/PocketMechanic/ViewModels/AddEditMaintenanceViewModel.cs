using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PocketMechanic.Models;
using PocketMechanic.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PocketMechanic.ViewModels
{
    [QueryProperty(nameof(VehicleId), "vehicleId")]
    [QueryProperty(nameof(MaintenanceId), "maintenanceId")]
    public partial class AddEditMaintenanceViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _vehicleId;

        [ObservableProperty]
        private int _maintenanceId;

        [ObservableProperty]
        private Vehicle _vehicle;

        [ObservableProperty]
        private ObservableCollection<MaintenanceType> _maintenanceTypes;

        [ObservableProperty]
        private MaintenanceType _selectedMaintenanceType;

        [ObservableProperty]
        private DateTime _datePerformed = DateTime.Today;

        [ObservableProperty]
        private int _mileageAtService;

        [ObservableProperty]
        private DateTime? _nextDueDate;

        [ObservableProperty]
        private int? _nextDueMileage;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private decimal? _cost;

        [ObservableProperty]
        private string _pageTitle = "Add Maintenance";

        [ObservableProperty]
        private bool _isEditMode;

        public AddEditMaintenanceViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _maintenanceTypes = new ObservableCollection<MaintenanceType>();
        }

        partial void OnVehicleIdChanged(int value)
        {
            if (value > 0)
            {
                Task.Run(async () => await LoadVehicleAndTypesAsync());
            }
        }

        partial void OnMaintenanceIdChanged(int value)
        {
            if (value > 0)
            {
                IsEditMode = true;
                PageTitle = "Edit Maintenance";
                Task.Run(async () => await LoadMaintenanceAsync());
            }
        }

        partial void OnSelectedMaintenanceTypeChanged(MaintenanceType value)
        {
            CalculateNextDue();
        }

        partial void OnDatePerformedChanged(DateTime value)
        {
            CalculateNextDue();
        }

        partial void OnMileageAtServiceChanged(int value)
        {
            CalculateNextDue();
        }

        private async Task LoadVehicleAndTypesAsync()
        {
            Vehicle = await _databaseService.GetVehicleAsync(VehicleId);
            if (Vehicle != null)
            {
                MileageAtService = Vehicle.CurrentMileage;
            }

            var types = await _databaseService.GetMaintenanceTypesAsync();
            MaintenanceTypes.Clear();
            foreach (var type in types)
            {
                MaintenanceTypes.Add(type);
            }
        }

        private async Task LoadMaintenanceAsync()
        {
            var record = await _databaseService.GetMaintenanceRecordAsync(MaintenanceId);
            if (record != null)
            {
                VehicleId = record.VehicleId;
                await LoadVehicleAndTypesAsync();

                var type = MaintenanceTypes.FirstOrDefault(t => t.Name == record.MaintenanceType);
                SelectedMaintenanceType = type;
                DatePerformed = record.DatePerformed;
                MileageAtService = record.MileageAtService;
                NextDueDate = record.NextDueDate;
                NextDueMileage = record.NextDueMileage;
                Notes = record.Notes;
                Cost = record.Cost;
            }
        }

        private void CalculateNextDue()
        {
            if (SelectedMaintenanceType == null)
                return;

            // Calculate next due date based on time interval
            if (SelectedMaintenanceType.DefaultMonthsInterval.HasValue)
            {
                NextDueDate = DatePerformed.AddMonths(SelectedMaintenanceType.DefaultMonthsInterval.Value);
            }

            // Calculate next due mileage based on mileage interval
            if (SelectedMaintenanceType.DefaultMileageInterval.HasValue)
            {
                NextDueMileage = MileageAtService + SelectedMaintenanceType.DefaultMileageInterval.Value;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedMaintenanceType == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a maintenance type", "OK");
                return;
            }

            if (MileageAtService <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid mileage", "OK");
                return;
            }

            var record = new MaintenanceRecord
            {
                Id = MaintenanceId,
                VehicleId = VehicleId,
                MaintenanceType = SelectedMaintenanceType.Name,
                DatePerformed = DatePerformed,
                MileageAtService = MileageAtService,
                NextDueDate = NextDueDate,
                NextDueMileage = NextDueMileage,
                Notes = Notes,
                Cost = Cost
            };

            await _databaseService.SaveMaintenanceRecordAsync(record);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
