using ParkingManagementSystem.Dtos;

namespace ReservationService.Services
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetAllAsync();
        Task<List<ReservationDto>> GetByUserAsync(int userId);
        Task<ReservationDto?> GetByIdAsync(int id);
        Task<ReservationDto> CreateAsync(int userId, CreateReservationDto dto);
        Task<bool> CancelAsync(int id, int userId);
        Task<int> CheckInAsync(int reservationId);
    }
}
