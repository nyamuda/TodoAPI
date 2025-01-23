namespace TodoAPI.Dtos.Item
{
    public class ItemAdminStatsDto
    {
        public int TotalItems { get; set; } = 0;

        public int TotalCompletedItems { get; set; } = 0;

        public int TotalPendingItems { get; set; } = 0;

        public int TotalCancelledItems { get; set; } = 0;

       
    }
}
