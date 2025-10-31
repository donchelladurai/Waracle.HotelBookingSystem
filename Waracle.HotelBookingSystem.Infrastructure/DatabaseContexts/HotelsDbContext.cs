using Microsoft.EntityFrameworkCore;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts
{
    public class HotelsDbContext : DbContext 
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public HotelsDbContext(DbContextOptions<HotelsDbContext> options) : base(options) { }

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
        }
    }
}
