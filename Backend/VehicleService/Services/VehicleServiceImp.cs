using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace VehicleService.Services
{
    public class VehicleServiceImp : IVehicleService
    {
          private readonly AppDbContext _context;

        public VehicleServiceImp(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    VehicleType = v.VehicleType,
                    UserId = v.UserId,
                    OwnerName = v.Owner.Name
                })
                .ToListAsync();
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.VehicleId == id)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    VehicleType = v.VehicleType,
                    UserId = v.UserId,
                    OwnerName = v.Owner.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByUserIdAsync(int userId)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.UserId == userId)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    VehicleType = v.VehicleType,
                    UserId = v.UserId,
                    OwnerName = v.Owner.Name
                })
                .ToListAsync();
        }

        public async Task<VehicleDto> AddVehicleAsync(VehicleDto vehicleDto)
        {
            var vehicle = new Vehicle
            {
                VehicleNumber = vehicleDto.VehicleNumber,
                VehicleType = vehicleDto.VehicleType,
                UserId = vehicleDto.UserId
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            vehicleDto.VehicleId = vehicle.VehicleId;

            return vehicleDto;
        }

        public async Task<bool> UpdateVehicleAsync(int id, UpdateVehicleDto vehicleDto)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
                return false;

            vehicle.VehicleNumber = vehicleDto.VehicleNumber;
            vehicle.VehicleType = vehicleDto.VehicleType;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}