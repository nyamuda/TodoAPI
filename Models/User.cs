using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; } = "User";

        public bool IsVerified { get; set; } = false;

        public List<Booking> Bookings { get; set; } = new List<Booking>();

        public List<Feedback> Feedback { get; set; } = new List<Feedback>();

    }
}
