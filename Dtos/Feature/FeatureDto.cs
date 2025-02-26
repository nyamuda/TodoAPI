using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Feature
{
    public class FeatureDto
    {
        [Required]
        public string Name { get; set; }

     
        public string? Description { get; set; }
    }
}
