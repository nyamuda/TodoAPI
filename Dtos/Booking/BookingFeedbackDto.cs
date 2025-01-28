using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class BookingFeedbackDto
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public int BookingId { get; set; }
    }
}
