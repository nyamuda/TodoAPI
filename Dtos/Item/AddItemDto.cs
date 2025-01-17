using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Item
{
    public class AddItemDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
