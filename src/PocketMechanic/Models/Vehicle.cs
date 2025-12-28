using SQLite;
using System;
using System.Collections.Generic;

namespace PocketMechanic.Models
{
    [Table("vehicles")]
    public class Vehicle
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Make { get; set; }

        [MaxLength(100)]
        public string Model { get; set; }

        public int Year { get; set; }

        [MaxLength(17)]
        public string VIN { get; set; }

        [MaxLength(20)]
        public string LicensePlate { get; set; }

        public int CurrentMileage { get; set; }

        public DateTime? PurchaseDate { get; set; }

        [MaxLength(100)]
        public string Nickname { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Ignore]
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();

        // Display name for UI
        [Ignore]
        public string DisplayName => string.IsNullOrWhiteSpace(Nickname) 
            ? $"{Year} {Make} {Model}" 
            : Nickname;
    }
}
