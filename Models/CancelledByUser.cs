using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Owned]
    public class CancelledByUser
    {
        public string Name { get; set; } = default!;
        public string Role { get; set; }=default!;

    }
}
