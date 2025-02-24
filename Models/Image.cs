namespace TodoAPI.Models
{
    public class Image
    {
        public int Id { get; set; }
        // Public URL for viewing
        //Used to display or access the image publicly.
        public string Url { get; set; } = default!;
        //Internal file path used for deletion or updating in Firebase Storage
        //We can use it if we want to delete the image on Firebase
        public string FilePath { get; set; } = default!;
        public string FileName { get; set; } = default!;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
