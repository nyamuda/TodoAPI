namespace TodoAPI.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public DateOnly YearFounded { get; set; } = default!;
    }
}
