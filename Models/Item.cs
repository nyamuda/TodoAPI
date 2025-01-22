namespace TodoAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string ServiceType { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime ScheduledAt { get; set; }

        public string AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; } // Nullable for guest bookings

        public User? User { get; set; } // Nullable for guest bookings

        //Guest fields for users not logged in
        public string GuestName { get; set; } = default!; 
        public string GuestEmail { get; set; } = default!;
        public string GuestPhone { get; set; } = default!;
        public string GuestLocation { get; set; } = default!;
    }
    

}
