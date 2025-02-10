using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    [Owned]
    public class CancelDetails
    {
        public string CancelReason { get; set; } = default!;

        public User CancelledBy { get; set; } =default!;
    }
}
