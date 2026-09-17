using ParkingManagementSystem.Dtos;

namespace InvoiceService.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();

        Task<IEnumerable<InvoiceDto>> GetMyInvoicesAsync(int userId);

        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);

        Task<InvoiceDto> GenerateInvoiceAsync(int paymentId);
    }
}