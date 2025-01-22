using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Item
{
    public class AddItemDto
    {
        [Required]
        public string VehicleType { get; set; }
        [Required]
        public string ServiceType { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public DateTime ScheduledAt { get; set; }

        public string AdditionalNotes { get; set; }
    }
}
