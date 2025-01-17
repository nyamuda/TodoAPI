using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Account
{
    public class EmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
