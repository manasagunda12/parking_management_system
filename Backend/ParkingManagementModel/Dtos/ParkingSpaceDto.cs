namespace ParkingManagementSystem.Dtos
{
    public class ParkingSpaceDto
    {
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public string SpaceType { get; set; }
        public string Status { get; set; }
        public int FloorNumber { get; set; }
        public bool HasChargingPort { get; set; }
        public int LotId { get; set; }
        public string LotName { get; set; }
    }
}
