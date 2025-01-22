using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Item
{
    public class AddGuestItemDto
    {
        [Required]
        public string GuestName{ get; set; }
        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; }
        [Required]
        public string GuestPhone { get; set; }
        [Required]
        public string VehicleType { get; set; }
        [Required]
        public string ServiceType { get; set; }
        [Required]
        public DateTime ScheduledAt { get; set; }

        public string AdditionalNotes { get; set; }

       
    }
}
