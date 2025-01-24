using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class UpdateBookingDto
    {
        [Required]
        public string Status { get; set; }

        [Required]
        public string VehicleType { get; set; }
        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public DateTime ScheduledAt { get; set; }

        public string AdditionalNotes { get; set; }
    }
}
