namespace TodoAPI.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string Url { get; set; } = default!;

        public string FileName { get; set; }=default!;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;

    }
}
