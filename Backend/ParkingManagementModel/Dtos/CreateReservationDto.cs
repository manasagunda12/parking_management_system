namespace ParkingManagementSystem.Dtos
{
    public class CreateReservationDto
    {
        public int VehicleId { get; set; }
        public int SpaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
