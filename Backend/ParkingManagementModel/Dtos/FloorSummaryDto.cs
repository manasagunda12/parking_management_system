namespace ParkingManagementSystem.Dtos
{
    public class FloorSummaryDto
    {
        public int FloorNumber { get; set; }
        public int TotalSpaces { get; set; }
        public int AvailableSpaces { get; set; }
        public int OccupiedSpaces { get; set; }
        public int ReservedSpaces { get; set; }
        public bool IsFull => AvailableSpaces == 0;
    }
}
