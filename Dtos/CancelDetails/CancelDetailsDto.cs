using TodoAPI.Models;

namespace TodoAPI.Dtos.CancelDetails
{
    public class CancelDetailsDto
    {
        public string CancelReason { get; set; } = default!;

        public DateTime CancelledAt { get; set; } = default!;

        public User CancelledByUser { get; set; } = default!;

        
        
    }
}
