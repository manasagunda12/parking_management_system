namespace ParkingManagementSystem.Dtos
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public string ClientSecret { get; set; } = string.Empty;

        public string PaymentIntentId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}