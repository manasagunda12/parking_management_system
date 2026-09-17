namespace ParkingManagementSystem.Dtos
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal ReservationFee { get; set; }
        public string Status { get; set; }
       
    }
}
