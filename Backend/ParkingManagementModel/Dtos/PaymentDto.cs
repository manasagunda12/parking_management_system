namespace ParkingManagementSystem.Dtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public int SessionId { get; set; }
    }
}
