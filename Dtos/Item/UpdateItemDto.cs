using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Item
{
    public class UpdateItemDto
    {
        [Required]
        public string Status { get; set; }
    }
}
