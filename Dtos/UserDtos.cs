using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //password must be at least 8 characters long
        //and must contain at least one uppercase letter, one lowercase letter, one number and one special character
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string Password { get; set; } 
    }

    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //password must be at least 8 characters long
        //and must contain at least one uppercase letter, one lowercase letter, one number and one special character
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string Password { get; set; }
    }
    public class UserUpdateDto {

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    
    }

  
