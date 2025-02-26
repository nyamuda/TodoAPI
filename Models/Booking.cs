

namespace TodoAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string VehicleType { get; set; } = default!; 
        public int ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int StatusId { get; set; }

        public Status Status { get; set; }

        public CancelDetails? CancelDetails { get; set; } //details about a cancelled booking
        public DateTime ScheduledAt { get; set; }

        public string? AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; } // Nullable for guest bookings

        public User? User { get; set; } // Nullable for guest bookings

        //Guest user for users not registered
       public GuestUser? GuestUser { get; set; }


        public Feedback? Feedback { get; set; }


    }
    

}
