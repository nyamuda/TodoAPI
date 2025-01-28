namespace TodoAPI.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int Rating { get; set; }

        public int BookingId { get; set; }

        public Booking Booking { get; set; }
    }
}
