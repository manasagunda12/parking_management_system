namespace ParkingManagementSystem.Dtos
{
    public class ParkingSessionDto
    {
        public int SessionId { get; set; }

        public int VehicleId { get; set; }

        public string VehicleNumber { get; set; } = string.Empty;

        public int SpaceId { get; set; }

        public string SpaceName { get; set; } = string.Empty;

        public string? EntryTime { get; set; }

        public string? ExitTime { get; set; }

        public double? Duration { get; set; }

        public decimal Amount { get; set; }
        public string Status{get;set;} = string.Empty;

    }
}