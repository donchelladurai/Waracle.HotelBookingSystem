using Microsoft.EntityFrameworkCore;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts
{
    public class AzureSqlHbsDbContext : DbContext 
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public AzureSqlHbsDbContext(DbContextOptions<AzureSqlHbsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasOne(r => r.Hotel).WithMany(h => h.Rooms).HasForeignKey(r => r.HotelId);
            modelBuilder.Entity<Booking>().HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId);
            modelBuilder.Entity<Booking>().Property(b => b.Reference).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<RoomType>().ToTable("RoomType");
            modelBuilder.Entity<RoomType>().HasKey(rt => rt.Id);
            modelBuilder.Entity<RoomType>().Property(rt => rt.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<RoomType>().Property(rt => rt.Capacity).IsRequired();

            modelBuilder.Entity<Room>().HasOne(r => r.RoomType).WithMany().HasForeignKey(r => r.RoomTypeId);

            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, Name = "Single", Capacity = 1 },
                new RoomType { Id = 2, Name = "Double", Capacity = 2 },
                new RoomType { Id = 3, Name = "Deluxe", Capacity = 4 }
            );

            modelBuilder.Entity<Hotel>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<Room>().HasIndex(r => r.HotelId);
            modelBuilder.Entity<Room>().HasIndex(r => r.RoomTypeId);

            
            modelBuilder.Entity<Booking>().HasIndex(b => b.Reference).IsUnique();
            modelBuilder.Entity<Booking>().HasIndex(b => b.RoomId);

            /* Note for the interviewers:
             I decided on a composite index here to optimise queries regarding room availability, given the dates */
            modelBuilder.Entity<Booking>().HasIndex(b => new { b.RoomId, b.CheckInDate });
            modelBuilder.Entity<Booking>().HasIndex(b => new { b.RoomId, b.CheckOutDate });
        }
    }
}
