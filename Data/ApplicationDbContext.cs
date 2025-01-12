
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<User> Users { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //A user can have many items and an item belongs to a user
            //This will create a one-to-many relationship between the User and Item entities
            //The Item entity will have a foreign key property called UserId
            modelBuilder.Entity<Item>()
                .HasOne(u => u.User)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
