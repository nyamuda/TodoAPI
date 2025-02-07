

namespace TodoAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string VehicleType { get; set; } = default!; 
        public int? ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int StatusId { get; set; }

        public Status Status { get; set; }
        public string? CancelReason { get; set; } //reason for cancelling a booking
        public DateTime ScheduledAt { get; set; }

        public string? AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; } // Nullable for guest bookings

        public User? User { get; set; } // Nullable for guest bookings

        //Guest fields for users not logged in
        public string? GuestName { get; set; }  
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; } 

        public Feedback? Feedback { get; set; }


    }
    

}
