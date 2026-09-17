using ParkingManagementSystem.Dtos;

namespace ParkingSessionService.Services
{
    public interface IParkingSessionService
    {
        Task<List<ParkingSessionDto>> GetAllSessionsAsync();

        Task<ParkingSessionDto?> GetSessionByIdAsync(
            int sessionId);

        Task<ParkingSessionDto> BookSlotAsync(
            int vehicleId,
            int spaceId);

        Task<bool> OccupySlotAsync(
            int sessionId);

        Task<ParkingSessionDto?> ExitVehicleAsync(
            int sessionId);
    }
}