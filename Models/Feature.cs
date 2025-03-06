using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Index(nameof(Name),IsUnique =true)]
    public class Feature
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public List<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
