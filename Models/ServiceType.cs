namespace TodoAPI.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }


        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
