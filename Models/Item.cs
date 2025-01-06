namespace TodoAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        public DateTime DueDate { get; set; }
    }

}
