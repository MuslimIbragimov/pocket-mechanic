using SQLite;
using Microsoft.Maui.Storage;
using PocketMechanic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PocketMechanic.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _databasePath;
        private bool _initialized = false;

        public DatabaseService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "pocketmechanic.db3");
        }

        private async Task InitializeAsync()
        {
            if (_initialized)
                return;

            _database = new SQLiteAsyncConnection(_databasePath);
            
            // Create tables
            await _database.CreateTableAsync<Vehicle>();
            await _database.CreateTableAsync<MaintenanceRecord>();
            await _database.CreateTableAsync<MaintenanceType>();

            // Seed predefined maintenance types
            await SeedMaintenanceTypesAsync();

            _initialized = true;
        }

        private async Task SeedMaintenanceTypesAsync()
        {
            var existingCount = await _database.Table<MaintenanceType>().CountAsync();
            if (existingCount == 0)
            {
                foreach (var type in MaintenanceType.PredefinedTypes)
                {
                    await _database.InsertAsync(type);
                }
            }
        }

        #region Vehicle Operations

        public async Task<List<Vehicle>> GetVehiclesAsync()
        {
            await InitializeAsync();
            return await _database.Table<Vehicle>().ToListAsync();
        }

        public async Task<Vehicle> GetVehicleAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<Vehicle>().Where(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveVehicleAsync(Vehicle vehicle)
        {
            await InitializeAsync();
            vehicle.UpdatedAt = DateTime.UtcNow;

            if (vehicle.Id != 0)
            {
                await _database.UpdateAsync(vehicle);
                return vehicle.Id;
            }
            else
            {
                vehicle.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(vehicle);
            }
        }

        public async Task<int> DeleteVehicleAsync(Vehicle vehicle)
        {
            await InitializeAsync();
            
            // Delete all associated maintenance records first
            await _database.ExecuteAsync("DELETE FROM maintenance_records WHERE VehicleId = ?", vehicle.Id);
            
            return await _database.DeleteAsync(vehicle);
        }

        public async Task<int> UpdateVehicleMileageAsync(int vehicleId, int newMileage)
        {
            await InitializeAsync();
            var vehicle = await GetVehicleAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.CurrentMileage = newMileage;
                vehicle.UpdatedAt = DateTime.UtcNow;
                return await _database.UpdateAsync(vehicle);
            }
            return 0;
        }

        #endregion

        #region Maintenance Record Operations

        public async Task<List<MaintenanceRecord>> GetMaintenanceRecordsAsync(int vehicleId)
        {
            await InitializeAsync();
            return await _database.Table<MaintenanceRecord>()
                .Where(mr => mr.VehicleId == vehicleId)
                .OrderByDescending(mr => mr.DatePerformed)
                .ToListAsync();
        }

        public async Task<MaintenanceRecord> GetMaintenanceRecordAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<MaintenanceRecord>().Where(mr => mr.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveMaintenanceRecordAsync(MaintenanceRecord record)
        {
            await InitializeAsync();

            if (record.Id != 0)
            {
                return await _database.UpdateAsync(record);
            }
            else
            {
                record.CreatedAt = DateTime.UtcNow;
                return await _database.InsertAsync(record);
            }
        }

        public async Task<int> DeleteMaintenanceRecordAsync(MaintenanceRecord record)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(record);
        }

        public async Task<List<MaintenanceRecord>> GetOverdueMaintenanceAsync(int vehicleId, int currentMileage)
        {
            await InitializeAsync();
            var allRecords = await GetMaintenanceRecordsAsync(vehicleId);
            var today = DateTime.Today;

            return allRecords.FindAll(mr =>
                (mr.NextDueDate.HasValue && mr.NextDueDate.Value < today) ||
                (mr.NextDueMileage.HasValue && mr.NextDueMileage.Value < currentMileage)
            );
        }

        public async Task<List<MaintenanceRecord>> GetDueSoonMaintenanceAsync(int vehicleId, int currentMileage)
        {
            await InitializeAsync();
            var allRecords = await GetMaintenanceRecordsAsync(vehicleId);
            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);

            return allRecords.FindAll(mr =>
                (mr.NextDueDate.HasValue && mr.NextDueDate.Value >= today && mr.NextDueDate.Value <= thirtyDaysFromNow) ||
                (mr.NextDueMileage.HasValue && mr.NextDueMileage.Value >= currentMileage && mr.NextDueMileage.Value <= currentMileage + 500)
            );
        }

        #endregion

        #region Maintenance Type Operations

        public async Task<List<MaintenanceType>> GetMaintenanceTypesAsync()
        {
            await InitializeAsync();
            return await _database.Table<MaintenanceType>().ToListAsync();
        }

        public async Task<MaintenanceType> GetMaintenanceTypeAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<MaintenanceType>().Where(mt => mt.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MaintenanceType> GetMaintenanceTypeByNameAsync(string name)
        {
            await InitializeAsync();
            return await _database.Table<MaintenanceType>().Where(mt => mt.Name == name).FirstOrDefaultAsync();
        }

        public async Task<int> SaveMaintenanceTypeAsync(MaintenanceType type)
        {
            await InitializeAsync();

            if (type.Id != 0)
            {
                return await _database.UpdateAsync(type);
            }
            else
            {
                type.IsCustom = true;
                return await _database.InsertAsync(type);
            }
        }

        public async Task<int> DeleteMaintenanceTypeAsync(MaintenanceType type)
        {
            await InitializeAsync();
            if (!type.IsCustom)
                throw new InvalidOperationException("Cannot delete predefined maintenance types");

            return await _database.DeleteAsync(type);
        }

        #endregion
    }
}
