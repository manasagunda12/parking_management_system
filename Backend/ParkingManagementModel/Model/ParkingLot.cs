using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class ParkingLot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LotId { get; set; }

        [Required(ErrorMessage = "Parking lot name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Location must be between 2 and 250 characters.")]
        public string Location { get; set; }
        public int NumberOfFloors { get; set; }

        [Required(ErrorMessage = "Total slots is required.")]
        [Range(1, 10000, ErrorMessage = "Total slots must be between 1 and 10000.")]
        public int TotalSlots { get; set; }

        [Range(0, 10000, ErrorMessage = "Available slots cannot be negative.")]
        public int AvailableSlots { get; set; }

        public ICollection<ParkingSpace> ParkingSpaces { get; set; } = new List<ParkingSpace>();
    }
}
