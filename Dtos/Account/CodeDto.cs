using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Account
{
    public class CodeDto
    {
        [Required]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 255 characters.")]
        public string Code { get; set; }
    }
}
