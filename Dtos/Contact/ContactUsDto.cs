using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Contact
{
    public class ContactUsDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Message { get; set; }
    }
}
