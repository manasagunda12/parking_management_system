using ParkingManagementSystem.Dtos;

namespace ParkingManagementSystem.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentDto>> GetAllAsync();

        Task<List<PaymentDto>> GetByUserIdAsync(int userId);

        Task<PaymentDto?> GetByIdAsync(int paymentId);

        Task<PaymentDto> CreateAsync(PaymentDto dto);

        Task<bool> MakePaymentAsync(
            int paymentId,
            string paymentMethod);
    }
}