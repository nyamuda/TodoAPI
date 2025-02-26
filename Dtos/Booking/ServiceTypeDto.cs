using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class ServiceTypeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public double Duration { get; set; }

        [Required]
        public string Overview { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int ImageId { get; set; }   
    }
}
