using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Owned]
    public class GuestUser
    {
        
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
