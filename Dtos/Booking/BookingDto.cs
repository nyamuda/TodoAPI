using System.Text.Json.Serialization;
using TodoAPI.Dtos.CancelDetails;
using TodoAPI.Models;
using TodoAPI.Dtos;
using TodoAPI.Dtos.User;
namespace TodoAPI.Dtos.Booking
{
    public class BookingDto
    {

        public int Id { get; set; }
        public string VehicleType { get; set; } = default!;
        public int ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int StatusId { get; set; }

        public Status Status { get; set; }

        public CancelDetailsDto? CancelDetails { get; set; } //details about a cancelled booking
        public DateTime ScheduledAt { get; set; }

        public string? AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; } // Nullable for guest bookings

        public UserDto? User { get; set; } // Nullable for guest bookings

        //Guest user for users not registered
        public GuestUser? GuestUser { get; set; }


        public Feedback? Feedback { get; set; }



    }
}
