using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace InvoiceService.Services
{
    public class InvoiceServiceImp : IInvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceServiceImp(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDto>> GetMyInvoicesAsync(int userId)
        {
            var result = await (
                from i in _context.Invoices
                join p in _context.Payments on i.PaymentId equals p.PaymentId
                join s in _context.ParkingSessions on p.SessionId equals s.SessionId
                join v in _context.Vehicles on s.VehicleId equals v.VehicleId
                join sp in _context.ParkingSpaces on s.SpaceId equals sp.SpaceId into spGroup
                from sp in spGroup.DefaultIfEmpty()
                join lot in _context.ParkingLots on sp.LotId equals lot.LotId into lotGroup
                from lot in lotGroup.DefaultIfEmpty()
                where v.UserId == userId
                select new InvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    PaymentId = i.PaymentId,
                    Date = i.Date,
                    TotalAmount = i.TotalAmount,
                    SpaceName = sp != null ? sp.SpaceName : null,
                    LotName = lot != null ? lot.Name : null,
                    PaymentMethod = p.PaymentMethod
                }
            ).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            return await (
                from i in _context.Invoices
                join p in _context.Payments on i.PaymentId equals p.PaymentId
                join s in _context.ParkingSessions on p.SessionId equals s.SessionId
                join sp in _context.ParkingSpaces on s.SpaceId equals sp.SpaceId into spGroup
                from sp in spGroup.DefaultIfEmpty()
                join lot in _context.ParkingLots on sp.LotId equals lot.LotId into lotGroup
                from lot in lotGroup.DefaultIfEmpty()
                select new InvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    PaymentId = i.PaymentId,
                    Date = i.Date,
                    TotalAmount = i.TotalAmount,
                    SpaceName = sp != null ? sp.SpaceName : null,
                    LotName = lot != null ? lot.Name : null,
                    PaymentMethod = p.PaymentMethod
                }
            ).ToListAsync();
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await (
                from i in _context.Invoices
                join p in _context.Payments on i.PaymentId equals p.PaymentId
                join s in _context.ParkingSessions on p.SessionId equals s.SessionId
                join sp in _context.ParkingSpaces on s.SpaceId equals sp.SpaceId into spGroup
                from sp in spGroup.DefaultIfEmpty()
                join lot in _context.ParkingLots on sp.LotId equals lot.LotId into lotGroup
                from lot in lotGroup.DefaultIfEmpty()
                where i.InvoiceId == invoiceId
                select new InvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    PaymentId = i.PaymentId,
                    Date = i.Date,
                    TotalAmount = i.TotalAmount,
                    SpaceName = sp != null ? sp.SpaceName : null,
                    LotName = lot != null ? lot.Name : null,
                    PaymentMethod = p.PaymentMethod
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new KeyNotFoundException("Payment not found.");

            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId);

            if (existingInvoice != null)
                throw new Exception("Invoice already exists for this payment.");

            var session = await _context.ParkingSessions
                .Include(s => s.Space)
                    .ThenInclude(sp => sp.ParkingLot)
                .FirstOrDefaultAsync(s => s.SessionId == payment.SessionId);

            var invoice = new Invoice
            {
                PaymentId = payment.PaymentId,
                Date = DateTime.Now,
                TotalAmount = payment.Amount
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                PaymentId = invoice.PaymentId,
                Date = invoice.Date,
                TotalAmount = invoice.TotalAmount,
                SpaceName = session?.Space?.SpaceName,
                LotName = session?.Space?.ParkingLot?.Name,
                PaymentMethod = payment.PaymentMethod
            };
        }
    }
}