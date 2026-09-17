using ParkingManagementSystem.Dtos;

namespace VehicleService.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task<IEnumerable<VehicleDto>> GetVehiclesByUserIdAsync(int userId);
        Task<VehicleDto> AddVehicleAsync(VehicleDto vehicleDto);
        Task<bool> UpdateVehicleAsync(int id, UpdateVehicleDto vehicleDto);
        Task<bool> DeleteVehicleAsync(int id);
    }
}