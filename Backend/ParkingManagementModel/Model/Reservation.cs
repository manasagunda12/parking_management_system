using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "VehicleId is required.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "SpaceId is required.")]
        public int SpaceId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Reservation fee cannot be negative.")]
        public decimal ReservationFee { get; set; }

        [Required]
        [RegularExpression("^(Confirmed|Cancelled|Completed)$", ErrorMessage = "Status must be Confirmed, Cancelled or Completed.")]
        public string Status { get; set; }

        public User User { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
        public ParkingSpace Space { get; set; } = null!;
    }
}
