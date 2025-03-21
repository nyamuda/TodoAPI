﻿
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; } = default!;

        public DbSet<User> Users { get; set; } = default!;


        public DbSet<ServiceType> ServiceTypes { get; set; } = default!;


        public DbSet<Feedback> Feedback { get; set; } = default!;

        public DbSet<Status> Statuses { get; set; } = default!;

        public DbSet<Image>  Images { get; set; } = default!;

        public DbSet<Feature> Features { get; set; } = default!;


        public DbSet<Company> Companies { get; set; } = default!;

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

            //there is a one-one relationship between Feedback and Booking
            //with the Feedback entity having the potential to be the child entity('many' side entity)
            //hence, the Feedback entity is the one with the foreign key (BookingId)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Booking)
                .WithOne(b => b.Feedback)
                .HasForeignKey<Feedback>(f => f.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if the primary entity which 'Booking' is deleted


            //A booking can have only one status and a status can have  many bookings (many bookings can have the same status)
            //Thus, there is a one-many relationship between Booking and Status
            modelBuilder.Entity<Status>()
                    .HasMany(s => s.Bookings)
                    .WithOne(b => b.Status)
                    .HasForeignKey(b => b.StatusId)
                    .OnDelete(DeleteBehavior.Cascade);



            //GuestUser Entity is owned by the Booking Entity
            //This is so because guest users are only allowed for those
            //users who are not registered but want to create booking
            modelBuilder.Entity<Booking>().OwnsOne(b => b.GuestUser);

            //there is a one-one relationship between Booking and CancelDetails
            //with the CancelDetails entity having the potential to be the child entity('many' side entity)
            //hence, the CancelDetails entity is the one with the foreign key (BookingId)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.CancelDetails)
                .WithOne(cd => cd.Booking)
                .HasForeignKey<CancelDetails>(cd => cd.BookingId)
                .OnDelete(DeleteBehavior.Cascade);  // Booking delete → CancelDetails delete


            //there is a one-one relationship between ServiceType and Image
            //with the ServiceType entity having the potential to be the child entity('many' side entity)
            //hence, the ServiceType entity is the one with the foreign key (ImageId)
            modelBuilder.Entity<ServiceType>()
                .HasOne(s=>s.Image)
                .WithOne()
                .HasForeignKey<ServiceType>(s=>s.ImageId)
                .OnDelete(DeleteBehavior.SetNull); // Image delete → ServiceType ImageId equal to Null

            //there is many-one relationship between CancelDetails and User
            modelBuilder.Entity<CancelDetails>()
                .HasOne(cd => cd.CancelledByUser)
                .WithMany()
                .HasForeignKey(cd => cd.CancelledByUserId)
                .OnDelete(DeleteBehavior.NoAction); //User delete → CancelDetails CancelledByUserId equal to null 

            //there is a many-to-many relationship between Feature and ServiceType
            //A car wash ServiceType can have many features and a Feature can exist in many service types
            modelBuilder.Entity<ServiceType>()
                .HasMany(s => s.Features)
                .WithMany(f => f.ServiceTypes);

            //There is a many-one relationship between ServiceType and Feedback
            //A ServiceType can have many feedback while a Feedback can only be for one ServiceType
            modelBuilder.Entity<Feedback>()
                 .HasOne(f => f.ServiceType)
                 .WithMany(s => s.Feedback)
                 .HasForeignKey(f => f.ServiceTypeId)
                 .OnDelete(DeleteBehavior.NoAction); //ServiceType delete → Feedback no action

            //There is a many-one relationship between User and Feedback
            //A User can have many feedback while a Feedback can only be from one User
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedback)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction); //User delete → Feedback no action


        }
    }
}
