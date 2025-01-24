namespace TodoAPI.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
