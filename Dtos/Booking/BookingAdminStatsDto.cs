namespace TodoAPI.Dtos.Booking
{
    public class BookingAdminStatsDto
    {
        public int TotalBookings { get; set; } = 0;

        public int TotalCompletedBookings { get; set; } = 0;

        public int TotalPendingBookings { get; set; } = 0;

        public int TotalCancelledBookings { get; set; } = 0;

       
    }
}
