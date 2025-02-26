namespace TodoAPI.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public string Content { get; set; } = default!;

        public int Rating { get; set; }

        public int BookingId { get; set; }

        public Booking? Booking { get; set; }


        public int ServiceTypeId { get; set; }  

        public ServiceType? ServiceType { get; set; } 
    }
}
