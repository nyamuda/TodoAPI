using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Images
{
    public class UpdateImageDto
    {

        [Required]
        public string Url { get; set; } = default!;

        [Required]
        public string FileName { get; set; } = default!;

        public string? Category { get; set; }

        public string? Description { get; set; }

       
    }
}
