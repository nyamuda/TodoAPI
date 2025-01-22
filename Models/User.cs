namespace TodoAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; } = "User";

        public bool IsVerified { get; set; } = false;

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
