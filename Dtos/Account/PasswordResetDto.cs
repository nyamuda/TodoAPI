// Ignore Spelling: Dtos Todo Dto

using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Account
{
    public class PasswordResetDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
