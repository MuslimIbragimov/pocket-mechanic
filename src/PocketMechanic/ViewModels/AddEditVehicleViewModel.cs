using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using PocketMechanic.Models;
using PocketMechanic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PocketMechanic.ViewModels
{
    [QueryProperty(nameof(VehicleId), "vehicleId")]
    public partial class AddEditVehicleViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly VehicleInfoService _vehicleInfoService;

        [ObservableProperty]
        private int _vehicleId;

        [ObservableProperty]
        private string _make;

        [ObservableProperty]
        private string _model;

        [ObservableProperty]
        private int _year = DateTime.Now.Year;

        [ObservableProperty]
        private List<MakeItem> _makes = new();

        [ObservableProperty]
        private MakeItem _selectedMake;

        [ObservableProperty]
        private List<string> _models = new();

        [ObservableProperty]
        private List<int> _years = new();

        [ObservableProperty]
        private bool _isLoadingMakes;

        [ObservableProperty]
        private bool _showMakePicker;

        [ObservableProperty]
        private bool _isLoadingModels;

        [ObservableProperty]
        private bool _showModelPicker;

        [ObservableProperty]
        private bool _showModelEntry;

        [ObservableProperty]
        private int _selectedMakeIndex = -1;

        [ObservableProperty]
        private int _selectedModelIndex = -1;

        [ObservableProperty]
        private int _selectedYearIndex = -1;

        [ObservableProperty]
        private VehicleRegion _currentRegion;

        [ObservableProperty]
        private string _regionFilterText;

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
        private string _imagePath;

        [ObservableProperty]
        private string _pageTitle = "Add Vehicle";

        [ObservableProperty]
        private bool _isEditMode;

        public AddEditVehicleViewModel(DatabaseService databaseService, VehicleInfoService vehicleInfoService)
        {
            _databaseService = databaseService;
            _vehicleInfoService = vehicleInfoService;
            
            // Initialize years synchronously
            Years = _vehicleInfoService.GetYears();
            
            // Set default year index
            SelectedYearIndex = Years.IndexOf(DateTime.Now.Year);

            // Auto-detect region based on user's location
            CurrentRegion = _vehicleInfoService.DetectRegion();
            UpdateRegionFilterText();
        }

        private void UpdateRegionFilterText()
        {
            RegionFilterText = CurrentRegion switch
            {
                VehicleRegion.NorthAmerica => "Showing: North American makes",
                VehicleRegion.Europe => "Showing: European makes",
                VehicleRegion.Asia => "Showing: Asian makes",
                VehicleRegion.All => "Showing: All makes worldwide",
                _ => ""
            };
        }

        public async Task InitializeAsync()
        {
            await LoadMakesAsync();
        }

        [RelayCommand]
        private void OpenMakePicker()
        {
            ShowMakePicker = true;
        }

        [RelayCommand]
        private void CloseMakePicker()
        {
            ShowMakePicker = false;
        }

        [RelayCommand]
        private async Task ChangeRegionAsync()
        {
            var action = await Shell.Current.DisplayActionSheet(
                "Can't find your make?",
                "Cancel",
                null,
                "North American makes",
                "European makes",
                "Asian makes",
                "All makes worldwide");

            if (action != "Cancel" && action != null)
            {
                CurrentRegion = action switch
                {
                    "North American makes" => VehicleRegion.NorthAmerica,
                    "European makes" => VehicleRegion.Europe,
                    "Asian makes" => VehicleRegion.Asia,
                    "All makes worldwide" => VehicleRegion.All,
                    _ => CurrentRegion
                };

                UpdateRegionFilterText();
                
                // Reload makes with new region
                SelectedMake = null;
                Make = null;
                Model = null;
                SelectedModelIndex = -1;
                Models = new List<string>();
                await LoadMakesAsync();
            }
        }

        private async Task LoadMakesAsync()
        {
            IsLoadingMakes = true;
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadMakesAsync: Starting, CurrentRegion={CurrentRegion}");
                var makes = await _vehicleInfoService.GetMakesWithLogosAsync(CurrentRegion);
                System.Diagnostics.Debug.WriteLine($"LoadMakesAsync: Received {makes?.Count ?? 0} makes");
                
                // Ensure we're on the main thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Makes = makes ?? new List<MakeItem>();
                    System.Diagnostics.Debug.WriteLine($"LoadMakesAsync: Makes.Count is now {Makes.Count}");
                    IsLoadingMakes = false;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadMakesAsync: Error - {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Makes = new List<MakeItem>();
                    IsLoadingMakes = false;
                });
            }
        }

        partial void OnSelectedMakeChanged(MakeItem value)
        {
            if (value != null)
            {
                Make = value.Name;
                Model = null;
                SelectedModelIndex = -1;
                Models = new List<string>();
                _ = LoadModelsAsync();
            }
        }

        partial void OnSelectedModelIndexChanged(int value)
        {
            if (value >= 0 && value < Models.Count)
            {
                Model = Models[value];
            }
        }

        partial void OnSelectedYearIndexChanged(int value)
        {
            if (value >= 0 && value < Years.Count)
            {
                Year = Years[value];
                
                // Reload models if make is already selected
                if (!string.IsNullOrWhiteSpace(Make))
                {
                    Model = null;
                    SelectedModelIndex = -1;
                    Models = new List<string>();
                    _ = LoadModelsAsync();
                }
            }
        }

        private async Task LoadModelsAsync()
        {
            if (string.IsNullOrWhiteSpace(Make))
                return;

            IsLoadingModels = true;
            ShowModelPicker = false;
            ShowModelEntry = false;
            
            var models = await _vehicleInfoService.GetModelsAsync(Make, Year);
            
            // Ensure we're on the main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Models = models;
                IsLoadingModels = false;
                
                // Show picker if we have models, otherwise show manual entry
                if (models != null && models.Count > 0)
                {
                    ShowModelPicker = true;
                    ShowModelEntry = false;
                }
                else
                {
                    ShowModelPicker = false;
                    ShowModelEntry = true;
                    Model = null;  // Clear any previous model selection
                }
            });
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
                Vin = vehicle.VIN;
                LicensePlate = vehicle.LicensePlate;
                CurrentMileage = vehicle.CurrentMileage;
                PurchaseDate = vehicle.PurchaseDate;
                Nickname = vehicle.Nickname;
                ImagePath = vehicle.ImagePath;

                // Set year first
                Year = vehicle.Year;
                SelectedYearIndex = Years.IndexOf(vehicle.Year);

                // Set make and wait for makes to load if needed
                if (Makes.Count == 0)
                    await LoadMakesAsync();
                
                Make = vehicle.Make;
                SelectedMake = Makes.FirstOrDefault(m => m.Name == vehicle.Make);

                // Load and set model
                await LoadModelsAsync();
                Model = vehicle.Model;
                SelectedModelIndex = Models.IndexOf(vehicle.Model);
            }
        }

        [RelayCommand]
        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Vehicle Photo"
                });

                if (result != null)
                {
                    // Save the image to app data directory
                    var fileName = $"vehicle_{Guid.NewGuid()}.jpg";
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    
                    using var stream = await result.OpenReadAsync();
                    using var fileStream = File.Create(filePath);
                    await stream.CopyToAsync(fileStream);
                    
                    ImagePath = filePath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
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
                Nickname = Nickname,
                ImagePath = ImagePath
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
