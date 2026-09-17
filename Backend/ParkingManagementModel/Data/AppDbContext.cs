using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Model;

namespace ParkingManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingLot> ParkingLots { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ParkingSession> ParkingSessions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Vehicles (one to many)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Owner)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Reservations (one to many)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle -> Reservations (one to many)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Reservations)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ParkingSpace -> Reservations (one to many)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Space)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ParkingLot -> ParkingSpaces (one to many)
            modelBuilder.Entity<ParkingSpace>()
                .HasOne(s => s.ParkingLot)
                .WithMany(f => f.ParkingSpaces)
                .HasForeignKey(s => s.LotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vehicle -> ParkingSessions (one to many)
            modelBuilder.Entity<ParkingSession>()
                .HasOne(ps => ps.Vehicle)
                .WithMany(v => v.ParkingSessions)
                .HasForeignKey(ps => ps.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ParkingSpace -> ParkingSessions (one to many)
            modelBuilder.Entity<ParkingSession>()
                .HasOne(ps => ps.Space)
                .WithMany(s => s.ParkingSessions)
                .HasForeignKey(ps => ps.SpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ParkingSession -> Payment (one to one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Session)
                .WithOne(ps => ps.Payment)
                .HasForeignKey<Payment>(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment -> Invoice (one to one)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Payment)
                .WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParkingSession>()
                .Property(ps => ps.Amount)
                .HasColumnType("decimal(18,2)");

            // Decimal precision
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationFee)
                .HasColumnType("decimal(18,2)");
        }
    }
}
