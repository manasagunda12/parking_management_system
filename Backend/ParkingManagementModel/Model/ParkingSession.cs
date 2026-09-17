using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class ParkingSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "VehicleId is required.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "SpaceId is required.")]
        public int SpaceId { get; set; }

        public DateTime? EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Duration cannot be negative.")]
        public double? Duration { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;


        public Vehicle Vehicle { get; set; } = null!;
        public ParkingSpace Space { get; set; } = null!;
        public Payment? Payment { get; set; }
    }
}
