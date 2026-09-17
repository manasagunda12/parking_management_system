using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace ParkingManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            return await _context.Payments
                .Select(x => new PaymentDto
                {
                    PaymentId = x.PaymentId,
                    Amount = x.Amount,
                    PaymentMethod = x.PaymentMethod,
                    Status = x.Status,
                    SessionId = x.SessionId
                })
                .ToListAsync();
        }

        public async Task<List<PaymentDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Join(_context.ParkingSessions,
                    p => p.SessionId,
                    s => s.SessionId,
                    (p, s) => new { p, s })
                .Join(_context.Vehicles,
                    ps => ps.s.VehicleId,
                    v => v.VehicleId,
                    (ps, v) => new { ps.p, v })
                .Where(x => x.v.UserId == userId)
                .Select(x => new PaymentDto
                {
                    PaymentId = x.p.PaymentId,
                    Amount = x.p.Amount,
                    PaymentMethod = x.p.PaymentMethod,
                    Status = x.p.Status,
                    SessionId = x.p.SessionId
                })
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetByIdAsync(
            int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(
                    x => x.PaymentId == paymentId);

            if (payment == null)
                return null;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                SessionId = payment.SessionId
            };
        }

        public async Task<PaymentDto> CreateAsync(
            PaymentDto dto)
        {
            var session = await _context.ParkingSessions
                .FirstOrDefaultAsync(
                    x => x.SessionId == dto.SessionId);

            if (session == null)
                throw new Exception(
                    "Parking session not found.");

            var payment = new Payment
            {
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Pending",
                SessionId = dto.SessionId
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            dto.PaymentId = payment.PaymentId;
            dto.Status = payment.Status;

            return dto;
        }

       public async Task<bool> MakePaymentAsync(int paymentId, string paymentMethod)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == paymentId);

            if (payment == null)
                return false;

            if (payment.Status == "Success")
                return false;

            payment.PaymentMethod = paymentMethod;
            payment.Status = "Success";

            // Auto-generate invoice
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId);

            if (existingInvoice == null)
            {
                _context.Invoices.Add(new Invoice
                {
                    PaymentId = payment.PaymentId,
                    Date = DateTime.Now,
                    TotalAmount = payment.Amount
                });
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}