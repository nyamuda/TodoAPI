namespace TodoAPI.Models
{
    public class Feature
    {
        public int Id { get; set; }

        public int Name { get; set; } = default!;
        public string? Description { get; set; }

        public List<ServiceType> ServiceTypes { get; set; }= new List<ServiceType>();
    }
}
