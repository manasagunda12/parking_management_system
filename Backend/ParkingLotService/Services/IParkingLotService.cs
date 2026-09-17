using ParkingManagementSystem.Dtos;

namespace ParkingService.Services
{
    public interface IParkingLotService
    {
        Task<List<ParkingLotDto>> GetAllAsync();
        Task<ParkingLotDto?> GetByIdAsync(int id);
        Task<ParkingLotDto> CreateAsync(ParkingLotDto dto);
        Task<bool> UpdateAsync(int id, ParkingLotDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
