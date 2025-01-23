using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Account
{
    public class UserUpdateDto
    {

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

    }
}
