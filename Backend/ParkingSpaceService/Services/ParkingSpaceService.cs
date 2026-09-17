using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace ParkingManagementSystem.Services
{
    public class ParkingSpaceServiceImp : IParkingSpaceService
    {
        private readonly AppDbContext _context;

        public ParkingSpaceServiceImp(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateSpacesAsync(
            int lotId,
            int totalSlots,
            int numberOfFloors)
        {
            var lot = await _context.ParkingLots
                .FirstOrDefaultAsync(x => x.LotId == lotId);

            if (lot == null)
                throw new Exception("Parking lot not found.");

            if (totalSlots <= 0)
                throw new Exception("Total slots must be greater than zero.");

            if (numberOfFloors <= 0)
                throw new Exception("Number of floors must be greater than zero.");

            if (totalSlots % numberOfFloors != 0)
                throw new Exception(
                    "Total slots must be equally divisible by number of floors.");

            bool spacesExist = await _context.ParkingSpaces
                .AnyAsync(x => x.LotId == lotId);

            if (spacesExist)
                throw new Exception(
                    "Parking spaces already exist for this parking lot.");

            int slotsPerFloor = totalSlots / numberOfFloors;

            for (int floor = 1; floor <= numberOfFloors; floor++)
            {
                string prefix;
                string spaceType;

                if (floor == 1)
                {
                    prefix = "A";
                    spaceType = "Car";
                }
                else if (floor == 2)
                {
                    prefix = "B";
                    spaceType = "Bike";
                }
                else
                {
                    prefix = $"F{floor}_";
                    spaceType = "Car";
                }

                for (int i = 1; i <= slotsPerFloor; i++)
                {
                    var parkingSpace = new ParkingSpace
                    {
                        SpaceName = $"{prefix}{i}",
                        SpaceType = spaceType,
                        Status = "Available",
                        FloorNumber = floor,
                        HasChargingPort = false,
                        LotId = lotId
                    };

                    _context.ParkingSpaces.Add(parkingSpace);
                }
            }

            lot.TotalSlots = totalSlots;
            lot.AvailableSlots = lot.TotalSlots;
            lot.NumberOfFloors = numberOfFloors;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ParkingSpaceDto>>
            GetSpacesByLotIdAsync(int lotId)
        {
            return await _context.ParkingSpaces
                .AsNoTracking()
                .Where(x => x.LotId == lotId)
                .OrderBy(x => x.FloorNumber)
                .ThenBy(x => x.SpaceName)
                .Select(x => new ParkingSpaceDto
                {
                    SpaceId = x.SpaceId,
                    SpaceName = x.SpaceName,
                    SpaceType = x.SpaceType,
                    Status = x.Status,
                    FloorNumber = x.FloorNumber,
                    HasChargingPort = x.HasChargingPort,
                    LotId = x.LotId,
                    LotName = x.ParkingLot.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingSpaceDto>>
            GetAvailableSpacesAsync(
                int lotId,
                string vehicleType)
        {
            return await _context.ParkingSpaces
                .AsNoTracking()
                .Where(x =>
                    x.LotId == lotId &&
                    x.Status == "Available" &&
                    x.SpaceType == vehicleType)
                .OrderBy(x => x.SpaceName)
                .Select(x => new ParkingSpaceDto
                {
                    SpaceId = x.SpaceId,
                    SpaceName = x.SpaceName,
                    SpaceType = x.SpaceType,
                    Status = x.Status,
                    FloorNumber = x.FloorNumber,
                    HasChargingPort = x.HasChargingPort,
                    LotId = x.LotId
                })
                .ToListAsync();
        }

        public async Task<ParkingSpaceDto?> GetSpaceByIdAsync(
            int spaceId)
        {
            var space = await _context.ParkingSpaces
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SpaceId == spaceId);

            if (space == null)
                return null;

            return new ParkingSpaceDto
            {
                SpaceId = space.SpaceId,
                SpaceName = space.SpaceName,
                SpaceType = space.SpaceType,
                Status = space.Status,
                FloorNumber = space.FloorNumber,
                HasChargingPort = space.HasChargingPort,
                LotId = space.LotId
            };
        }

        public async Task<bool> UpdateSpaceStatusAsync(
            int spaceId,
            string status)
        {
            string[] validStatuses =
            {
                "Available",
                "Reserved",
                "Occupied"
            };

            if (!validStatuses.Contains(status))
                return false;

            var space = await _context.ParkingSpaces
                .FirstOrDefaultAsync(x => x.SpaceId == spaceId);

            if (space == null)
                return false;

            string oldStatus = space.Status;

            if (status == "Reserved" &&
                oldStatus != "Available")
                return false;

            if (status == "Occupied" &&
                oldStatus != "Reserved")
                return false;

            if (status == "Available" &&
                oldStatus == "Available")
                return false;

            space.Status = status;

            var lot = await _context.ParkingLots
                .FirstOrDefaultAsync(x => x.LotId == space.LotId);

            if (lot != null)
            {
                if (oldStatus == "Available" &&
                    status != "Available")
                {
                    lot.AvailableSlots--;
                }
                else if (oldStatus != "Available" &&
                         status == "Available")
                {
                    lot.AvailableSlots++;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}