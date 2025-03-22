using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{

    public class CancelDetails
    {
        public int Id { get; set; }
        public string CancelReason { get; set; } = default!;

        public DateTime CancelledAt { get; set; } = DateTime.Now;

        public int? CancelledByUserId { get; set; } =default!;

        public User? CancelledByUser { get; set; } = default!;

        public int BookingId { get; set; }=default!;
        public Booking Booking { get; set; }=default!;
    }
}
