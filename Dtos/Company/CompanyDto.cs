using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Company
{
    public class CompanyDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Address { get; set; } = default!;
        [Required]
        public string Phone { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public DateTime DateFounded { get; set; } = default!;
    }
}
