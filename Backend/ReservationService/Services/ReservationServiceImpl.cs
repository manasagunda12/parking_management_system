using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Dtos;
using ParkingManagementSystem.Model;
using System.Data;

namespace ReservationService.Services
{
    public class ReservationServiceImpl : IReservationService
    {
        private readonly AppDbContext _context;
        private const decimal RatePerHour = 50m;

        public ReservationServiceImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationDto>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Include(r => r.Space)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<List<ReservationDto>> GetByUserAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Include(r => r.Space)
                .Where(r => r.UserId == userId)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<ReservationDto?> GetByIdAsync(int id)
        {
            var r = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Include(r => r.Space)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            return r == null ? null : MapToDto(r);
        }

        public async Task<ReservationDto> CreateAsync(int userId, CreateReservationDto dto)
        {
            var now = DateTime.Now;

            if (dto.StartTime <= now)
                throw new ArgumentException("Start time must be in the future.");

            if (dto.StartTime > now.AddDays(1))
                throw new ArgumentException("Reservation can only be made up to 1 day in advance.");

            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time.");

            // Begin transaction to prevent double booking
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Lock the space row for update — prevents other transactions from reading/modifying it
                var space = await _context.ParkingSpaces
                    .FromSqlRaw("SELECT * FROM ParkingSpaces WITH (UPDLOCK, ROWLOCK) WHERE SpaceId = {0}", dto.SpaceId)
                    .FirstOrDefaultAsync();

                if (space == null) throw new KeyNotFoundException("Parking space not found.");
                if (space.Status != "Available") throw new InvalidOperationException("Parking space is not available.");

                var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
                if (vehicle == null) throw new KeyNotFoundException("Vehicle not found.");
                if (vehicle.UserId != userId) throw new UnauthorizedAccessException("Vehicle does not belong to this user.");

                var hours = (decimal)(dto.EndTime - dto.StartTime).TotalHours;
                var fee = Math.Round(hours * RatePerHour, 2);

                var reservation = new Reservation
                {
                    UserId = userId,
                    VehicleId = dto.VehicleId,
                    SpaceId = dto.SpaceId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    ReservationFee = fee,
                    Status = "Confirmed"
                };

                // Only mark space Reserved if start time is now or very soon (within 30 min)
                if (dto.StartTime <= now.AddMinutes(30))
                    space.Status = "Reserved";

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var user = await _context.Users.FindAsync(userId);

                return new ReservationDto
                {
                    ReservationId = reservation.ReservationId,
                    UserId = userId,
                    UserName = user!.Name,
                    VehicleId = dto.VehicleId,
                    VehicleNumber = vehicle.VehicleNumber,
                    SpaceId = dto.SpaceId,
                    SpaceName = space.SpaceName,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    ReservationFee = fee,
                    Status = reservation.Status
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CheckInAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Space)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");

            if (reservation.Status != "Confirmed")
                throw new InvalidOperationException("Reservation is not in Confirmed status.");

            // Check if session already exists for this reservation
            var existingSession = await _context.ParkingSessions
                .FirstOrDefaultAsync(s =>
                    s.VehicleId == reservation.VehicleId &&
                    s.SpaceId == reservation.SpaceId &&
                    s.Status == "Occupied");

            if (existingSession != null)
                return existingSession.SessionId;

            // Mark space Occupied
            reservation.Space.Status = "Occupied";
            reservation.Status = "Completed";

            var lot = await _context.ParkingLots
                .FirstOrDefaultAsync(l => l.LotId == reservation.Space.LotId);

            if (lot != null && reservation.Space.Status == "Available")
                lot.AvailableSlots--;

            // Create parking session
            var session = new ParkingSession
            {
                VehicleId = reservation.VehicleId,
                SpaceId = reservation.SpaceId,
                EntryTime = DateTime.Now,
                Status = "Occupied"
            };

            _context.ParkingSessions.Add(session);
            await _context.SaveChangesAsync();

            return session.SessionId;
        }

        public async Task<bool> CancelAsync(int id, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Space)
                    .FirstOrDefaultAsync(r => r.ReservationId == id);

                if (reservation == null) return false;
                if (reservation.UserId != userId) throw new UnauthorizedAccessException("Unauthorized to cancel this reservation.");
                if (reservation.Status == "Cancelled") throw new InvalidOperationException("Reservation is already cancelled.");

                reservation.Status = "Cancelled";

                // Only free the space if it was actually Reserved (future reservations leave it Available)
                if (reservation.Space.Status == "Reserved")
                    reservation.Space.Status = "Available";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static ReservationDto MapToDto(Reservation r) => new ReservationDto
        {
            ReservationId = r.ReservationId,
            UserId = r.UserId,
            UserName = r.User.Name,
            VehicleId = r.VehicleId,
            VehicleNumber = r.Vehicle.VehicleNumber,
            SpaceId = r.SpaceId,
            SpaceName = r.Space.SpaceName,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            ReservationFee = r.ReservationFee,
            Status = r.Status
        };
    }
}
