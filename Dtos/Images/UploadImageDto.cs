using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Images
{
    //Dto for uploading an image to firebase
    public class UploadImageDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
        public string? Category { get; set; }

        public string? Description { get; set; }



    }
}
