using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class BookingStatusUpdateDto
    {
        [Required]
        public string StatusName { get; set; }
        
        //if status is changed to "cancelled"
        public string? CancelReason { get; set; }
    }
}
