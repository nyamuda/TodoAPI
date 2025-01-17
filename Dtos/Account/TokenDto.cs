using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Account
{
    public class TokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
