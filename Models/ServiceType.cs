namespace TodoAPI.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Overview { get; set; } = default!;

        public double Duration { get; set; }=default!;

        public string Description { get; set; }=default!;

        public int? ImageId { get; set; }   

        public Image? Image { get; set; }

       
        public double Price { get; set; }

        public List<Feature> Features { get; set; } = new List<Feature>();

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
