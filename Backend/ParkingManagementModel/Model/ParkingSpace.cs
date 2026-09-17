using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class ParkingSpace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpaceId { get; set; }

        [Required(ErrorMessage = "Space name is required.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Space name must be between 1 and 20 characters.")]
        public string SpaceName { get; set; }

        [Required(ErrorMessage = "Space type is required.")]
        [RegularExpression("^(Car|Bike|EV)$", ErrorMessage = "Space type must be Car, Bike or EV.")]
        public string SpaceType { get; set; }

        [Required]
        [RegularExpression("^(Available|Occupied|Reserved)$", ErrorMessage = "Status must be Available, Occupied or Reserved.")]
        public string Status { get; set; } = "Available";

        [Range(0, 100, ErrorMessage = "Floor number must be between 0 and 100.")]
        public int FloorNumber { get; set; }

        public bool HasChargingPort { get; set; } = false;

        [Required(ErrorMessage = "LotId is required.")]
        public int LotId { get; set; }

        public ParkingLot ParkingLot { get; set; } = null!;
        public ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
