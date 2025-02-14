namespace TodoAPI.Dtos.Booking
{
    public class BookingUserStatsDto
    {
        public int TotalBookings { get; set; } = 0;

        public int TotalCompletedBookings { get; set; } = 0;
 
        public int TotalConfirmedBookings { get; set; } = 0;

        public int TotalPendingBookings { get; set; } = 0;

        public int TotalCancelledBookings { get; set; } = 0; 
    }
}
