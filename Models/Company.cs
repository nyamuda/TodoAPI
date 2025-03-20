// Ignore Spelling: Facebook Instagram

using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;

        public DateTime DateFounded { get; set; } = default!;
         
        public string? LinkedInUrl { get; set; }

        public string? FacebookUrl { get; set; }

        public string? InstagramUrl { get; set; } 

        public string? OpeningHours { get; set; } = "Monday - Friday: 09.00 AM - 05.00 PM";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
