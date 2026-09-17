using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace ParkingService.Services
{
    public class ParkingLotService : IParkingLotService
    {
        private readonly AppDbContext _context;

        public ParkingLotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingLotDto>> GetAllAsync()
        {
            return await _context.ParkingLots
                .Include(l => l.ParkingSpaces)
                .Select(l => new ParkingLotDto
                {
                    LotId = l.LotId,
                    Name = l.Name,
                    Location = l.Location,
                    TotalSlots = l.TotalSlots,
                    AvailableSlots = l.ParkingSpaces.Count(s => s.Status == "Available"),
                    NumberOfFloors = l.NumberOfFloors
                })
                .ToListAsync();
        }

        public async Task<ParkingLotDto?> GetByIdAsync(int id)
        {
            var lot = await _context.ParkingLots
                .Include(l => l.ParkingSpaces)
                .FirstOrDefaultAsync(l => l.LotId == id);

            if (lot == null)
                return null;

            return new ParkingLotDto
            {
                LotId = lot.LotId,
                Name = lot.Name,
                Location = lot.Location,
                TotalSlots = lot.TotalSlots,
                AvailableSlots = lot.ParkingSpaces.Count(s => s.Status == "Available"),
                NumberOfFloors = lot.NumberOfFloors
            };
        }

        public async Task<ParkingLotDto> CreateAsync(ParkingLotDto dto)
        {
            var lot = new ParkingLot
            {
                Name = dto.Name,
                Location = dto.Location,
                TotalSlots = dto.TotalSlots,
                AvailableSlots = dto.TotalSlots,
                NumberOfFloors = dto.NumberOfFloors
            };

            _context.ParkingLots.Add(lot);

            await _context.SaveChangesAsync();

            return new ParkingLotDto
            {
                LotId = lot.LotId,
                Name = lot.Name,
                Location = lot.Location,
                TotalSlots = lot.TotalSlots,
                AvailableSlots = lot.AvailableSlots,
                NumberOfFloors = lot.NumberOfFloors
            };
        }

        public async Task<bool> UpdateAsync(int id, ParkingLotDto dto)
        {
            var lot = await _context.ParkingLots.FindAsync(id);

            if (lot == null)
                return false;

            lot.Name = dto.Name;
            lot.Location = dto.Location;
            lot.TotalSlots = dto.TotalSlots;
            lot.NumberOfFloors = dto.NumberOfFloors;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lot = await _context.ParkingLots.FindAsync(id);

            if (lot == null)
                return false;

            _context.ParkingLots.Remove(lot);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}