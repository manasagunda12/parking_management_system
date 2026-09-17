using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Model
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression("^(Cash|Card|UPI|NetBanking)$", ErrorMessage = "Payment method must be Cash, Card, UPI or NetBanking.")]
        public string PaymentMethod { get; set; }

        [Required]
        [RegularExpression("^(Pending|Success|Completed|Failed)$", ErrorMessage = "Status must be Pending, Success, Completed or Failed.")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "SessionId is required.")]
        public int SessionId { get; set; }

        public ParkingSession Session { get; set; } = null!;
        public Invoice? Invoice { get; set; }
    }
}
