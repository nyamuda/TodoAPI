using System.Text.Json.Serialization;
using TodoAPI.Models;

namespace TodoAPI.Dtos.Booking
{
    public class BookingDto
    {

        public int Id { get; set; }
        public string VehicleType { get; set; } = default!;
        public int? ServiceTypeId { get; set; }
        public string Location { get; set; } = default!;
        public string Status { get; set; } 

        public string? CancelReason { get; set; } //reason for cancelling a booking
        public DateTime ScheduledAt { get; set; }

        public string? AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; } 

       
    }
}
