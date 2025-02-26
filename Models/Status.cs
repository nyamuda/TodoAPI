using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Index(nameof(Name),IsUnique =true)]
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Booking> Bookings { get; set; }
    }
}
