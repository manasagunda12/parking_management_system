namespace ParkingManagementSystem.Dtos
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int PaymentId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SpaceName { get; set; }
        public string? LotName { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
