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

        public DateOnly DateFounded { get; set; } = default!;

    }
}
