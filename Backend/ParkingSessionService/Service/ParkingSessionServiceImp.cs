using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;

namespace ParkingSessionService.Services
{
    public class ParkingSessionServiceImp : IParkingSessionService
    {
        private readonly AppDbContext _context;

        public ParkingSessionServiceImp(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingSessionDto>> GetAllSessionsAsync()
        {
            var sessions = await _context.ParkingSessions
                .Include(x => x.Vehicle)
                .Include(x => x.Space)
                .ToListAsync();

            return sessions.Select(x => new ParkingSessionDto
            {
                SessionId = x.SessionId,
                VehicleId = x.VehicleId,
                VehicleNumber = x.Vehicle.VehicleNumber,
                SpaceId = x.SpaceId,
                SpaceName = x.Space.SpaceName,
                EntryTime = x.EntryTime.HasValue ? x.EntryTime.Value.ToString("hh:mm tt") : null,
                ExitTime = x.ExitTime.HasValue ? x.ExitTime.Value.ToString("hh:mm tt") : null,
                Duration = x.Duration,
                Amount = x.Amount,
                Status = x.Status,
            }).ToList();
        }

        public async Task<ParkingSessionDto?> GetSessionByIdAsync(int sessionId)
        {
            var session = await _context.ParkingSessions
                .Include(x => x.Vehicle)
                .Include(x => x.Space)
                .FirstOrDefaultAsync(x => x.SessionId == sessionId);

            if (session == null)
                return null;

            return new ParkingSessionDto
            {
                SessionId = session.SessionId,
                VehicleId = session.VehicleId,
                VehicleNumber = session.Vehicle.VehicleNumber,
                SpaceId = session.SpaceId,
                SpaceName = session.Space.SpaceName,
                EntryTime = session.EntryTime.HasValue ? session.EntryTime.Value.ToString("hh:mm tt") : null,
                ExitTime = session.ExitTime.HasValue ? session.ExitTime.Value.ToString("hh:mm tt") : null,
                Duration = session.Duration,
                Amount = session.Amount,
                Status = session.Status
            };
        }

        // Available -> Reserved
        public async Task<ParkingSessionDto> BookSlotAsync(int vehicleId, int spaceId)
{
    var vehicle = await _context.Vehicles
        .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

    if (vehicle == null)
        throw new ArgumentException("Vehicle not found.");

    // Check for existing active booking
    var activeSession = await _context.ParkingSessions
        .FirstOrDefaultAsync(s =>
            s.VehicleId == vehicleId &&
            (s.Status == "Reserved" || s.Status == "Occupied"));

    if (activeSession != null)
        throw new ArgumentException(
            $"Vehicle already has an active booking (Session ID: {activeSession.SessionId}).");

    var space = await _context.ParkingSpaces
        .FirstOrDefaultAsync(s => s.SpaceId == spaceId);

    if (space == null)
        throw new ArgumentException("Parking space not found.");

    if (space.Status != "Available")
        throw new ArgumentException(
            $"Slot {space.SpaceName} is not available.");

    // Update space status
    space.Status = "Reserved";

    var lot = await _context.ParkingLots
        .FirstOrDefaultAsync(l => l.LotId == space.LotId);

    if (lot != null)
        lot.AvailableSlots--;

    // Create booking session
    var session = new ParkingSession
    {
        VehicleId = vehicleId,
        SpaceId = spaceId,
        Status = "Reserved"
    };

    _context.ParkingSessions.Add(session);

    await _context.SaveChangesAsync();

    return new ParkingSessionDto
    {
        SessionId = session.SessionId,
        VehicleId = vehicle.VehicleId,
        VehicleNumber = vehicle.VehicleNumber,
        SpaceId = space.SpaceId,
        SpaceName = space.SpaceName,
        Status = session.Status
    };
}

        // Reserved -> Occupied
        public async Task<bool> OccupySlotAsync(int sessionId)
        {
            var session = await _context.ParkingSessions
                .FirstOrDefaultAsync(x => x.SessionId == sessionId);

            if (session == null)
                return false;

            // If session already has ExitTime it is completed
            if (session.ExitTime != null)
                return false;

            var space = await _context.ParkingSpaces
                .FirstOrDefaultAsync(x => x.SpaceId == session.SpaceId);

            if (space == null)
                return false;

            session.EntryTime = DateTime.Now;
            session.Status = "Occupied";
            space.Status = "Occupied";

            await _context.SaveChangesAsync();

            return true;
        }

        // Occupied -> Completed
        public async Task<ParkingSessionDto?> ExitVehicleAsync(int sessionId)
        {
            var session = await _context.ParkingSessions
                .Include(x => x.Vehicle)
                .Include(x => x.Space)
                .FirstOrDefaultAsync(x => x.SessionId == sessionId);

            if (session == null)
                return null;

            // Ensure session has been occupied (EntryTime set) and not already exited
            if (session.EntryTime == null || session.ExitTime != null)
                return null;

            session.ExitTime = DateTime.Now;

            var entryTime = session.EntryTime ?? session.ExitTime!.Value.AddHours(-1);
            TimeSpan parkedDuration = session.ExitTime!.Value - entryTime;

            double billedHours = Math.Ceiling(parkedDuration.TotalHours);
            double actualHours = Math.Round(parkedDuration.TotalHours, 2);

            decimal hourlyRate = 20;
            decimal amount = (decimal)billedHours * hourlyRate;

            session.Duration = actualHours;
            session.Amount = amount;
            session.Status = "Completed";
            session.Space.Status = "Available";

            var lot = await _context.ParkingLots
                .FirstOrDefaultAsync(x => x.LotId == session.Space.LotId);

            if (lot != null)
                lot.AvailableSlots++;

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(x => x.SessionId == sessionId);

            if (existingPayment == null)
            {
                _context.Payments.Add(new Payment
                {
                    SessionId = session.SessionId,
                    Amount = amount,
                    PaymentMethod = "Cash",
                    Status = "Pending"
                });
            }

            await _context.SaveChangesAsync();

            return new ParkingSessionDto
            {
                SessionId = session.SessionId,
                VehicleId = session.VehicleId,
                VehicleNumber = session.Vehicle.VehicleNumber,
                SpaceId = session.SpaceId,
                SpaceName = session.Space.SpaceName,
                EntryTime = entryTime.ToString("hh:mm tt"),
                ExitTime = session.ExitTime.Value.ToString("hh:mm tt"),
                Duration = actualHours,
                Amount = amount
            };
        }
    }
}
