namespace ParkingManagementSystem.Dtos
{
    public class ParkingLotDto
    {
        public int LotId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int TotalSlots { get; set; }
        public int NumberOfFloors { get; set; }
        public int AvailableSlots { get; set; }
    }
}
