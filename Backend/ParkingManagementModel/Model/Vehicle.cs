using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Vehicle number is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Vehicle number must be between 2 and 20 characters.")]
        public string VehicleNumber { get; set; }

        [Required(ErrorMessage = "Vehicle type is required.")]
        [RegularExpression("^(Car|Bike|EV)$", ErrorMessage = "Vehicle type must be Car, Bike or EV.")]
        public string VehicleType { get; set; }

        [Required]
        public int UserId { get; set; }

        public User Owner { get; set; } = null!;
        public ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
