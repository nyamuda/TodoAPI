using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Images
{
    //Dto for adding image information to the database
    public class AddImageDto
    {

        [Required]
        public string Url { get; set; } = default!;

        [Required]
        public string FileName { get; set; } = default!;

        public string? Category { get; set; }

        public string? Description { get; set; }

        
    }
}
