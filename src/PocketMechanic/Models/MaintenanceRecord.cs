using SQLite;
using System;

namespace PocketMechanic.Models
{
    [Table("maintenance_records")]
    public class MaintenanceRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int VehicleId { get; set; }

        [MaxLength(100)]
        public string MaintenanceType { get; set; }

        public DateTime DatePerformed { get; set; }

        public int MileageAtService { get; set; }

        public DateTime? NextDueDate { get; set; }

        public int? NextDueMileage { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public decimal? Cost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Ignore]
        public Vehicle Vehicle { get; set; }

        // Helper property to determine if maintenance is overdue
        [Ignore]
        public bool IsOverdue
        {
            get
            {
                if (NextDueDate.HasValue && NextDueDate.Value < DateTime.Today)
                    return true;
                // Note: We'd need current mileage from vehicle to check mileage-based overdue
                return false;
            }
        }

        // Helper property to determine if maintenance is due soon
        [Ignore]
        public bool IsDueSoon
        {
            get
            {
                if (NextDueDate.HasValue)
                {
                    var daysUntilDue = (NextDueDate.Value - DateTime.Today).TotalDays;
                    if (daysUntilDue >= 0 && daysUntilDue <= 30)
                        return true;
                }
                return false;
            }
        }
    }
}
