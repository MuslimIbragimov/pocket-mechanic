using SQLite;

namespace PocketMechanic.Models
{
    [Table("maintenance_types")]
    public class MaintenanceType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Unique]
        public string Name { get; set; }

        // Default interval in miles
        public int? DefaultMileageInterval { get; set; }

        // Default interval in months
        public int? DefaultMonthsInterval { get; set; }

        public bool IsCustom { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // Predefined maintenance types
        public static readonly MaintenanceType[] PredefinedTypes = new[]
        {
            new MaintenanceType { Name = "Oil Change", DefaultMileageInterval = 5000, DefaultMonthsInterval = 6, Description = "Regular engine oil change" },
            new MaintenanceType { Name = "Oil Filter", DefaultMileageInterval = 5000, DefaultMonthsInterval = 6, Description = "Oil filter replacement" },
            new MaintenanceType { Name = "Air Filter", DefaultMileageInterval = 15000, DefaultMonthsInterval = 12, Description = "Engine air filter replacement" },
            new MaintenanceType { Name = "Cabin Air Filter", DefaultMileageInterval = 15000, DefaultMonthsInterval = 12, Description = "Cabin air filter replacement" },
            new MaintenanceType { Name = "Tire Rotation", DefaultMileageInterval = 5000, DefaultMonthsInterval = 6, Description = "Rotate tires for even wear" },
            new MaintenanceType { Name = "Wiper Blades", DefaultMileageInterval = null, DefaultMonthsInterval = 12, Description = "Replace windshield wiper blades" },
            new MaintenanceType { Name = "Battery", DefaultMileageInterval = null, DefaultMonthsInterval = 48, Description = "Battery replacement" },
            new MaintenanceType { Name = "Spark Plugs", DefaultMileageInterval = 30000, DefaultMonthsInterval = null, Description = "Spark plug replacement" },
            new MaintenanceType { Name = "Coolant Flush", DefaultMileageInterval = 30000, DefaultMonthsInterval = 24, Description = "Engine coolant flush and fill" },
            new MaintenanceType { Name = "Brake Fluid", DefaultMileageInterval = null, DefaultMonthsInterval = 24, Description = "Brake fluid flush and fill" },
            new MaintenanceType { Name = "Power Steering Fluid", DefaultMileageInterval = null, DefaultMonthsInterval = 12, Description = "Check and fill power steering fluid" },
            new MaintenanceType { Name = "Transmission Fluid", DefaultMileageInterval = 30000, DefaultMonthsInterval = null, Description = "Transmission fluid change" }
        };
    }
}
