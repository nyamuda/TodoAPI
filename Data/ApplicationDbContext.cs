
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; } = default!;

        public DbSet<User> Users { get; set; } = default!;


        public DbSet<ServiceType> ServiceTypes { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //A user can have many bookings and an booking belongs to a user
            //This will create a one-to-many relationship between the User and Booking entities
            //The Booking entity will have a foreign key property called UserId
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //A service type can have many bookings a
            //and an booking can only have one service type
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServiceType)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
