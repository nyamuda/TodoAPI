using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class StatusDto
    {
        [Required]
        public string Name { get; set; }
    }
}
