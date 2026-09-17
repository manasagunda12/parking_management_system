using ParkingManagementSystem.Dtos;

namespace ParkingManagementSystem.Services
{
    public interface IParkingSpaceService
    {
        Task CreateSpacesAsync(
            int lotId,
            int totalSlots,
            int numberOfFloors);

        Task<IEnumerable<ParkingSpaceDto>>
            GetSpacesByLotIdAsync(int lotId);

        Task<IEnumerable<ParkingSpaceDto>>
            GetAvailableSpacesAsync(
                int lotId,
                string vehicleType);

        Task<ParkingSpaceDto?>
            GetSpaceByIdAsync(int spaceId);

        Task<bool>
            UpdateSpaceStatusAsync(
                int spaceId,
                string status);
    }
}