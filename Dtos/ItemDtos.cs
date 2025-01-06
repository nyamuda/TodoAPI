using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos
{
    public class AddItemDto
    {
        [Required]
        public string Title { get; set; }

        
        public bool IsCompleted { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public class GetItemDto
    {
    }

    public class UpdateItemDto
    {
        public string Title { get; set; }


        public bool IsCompleted { get; set; }

        
        public string Description { get; set; }

       
        public DateTime DueDate { get; set; }
    }
}
