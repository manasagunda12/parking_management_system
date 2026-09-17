namespace ParkingManagementSystem.Dtos
{
    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public int UserId { get; set; }
        public string? OwnerName { get; set; }
    }
}
