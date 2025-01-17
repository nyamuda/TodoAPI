namespace TodoAPI.Dtos.Item
{
    public class ItemUserStatsDto
    {
        public int TotalItems { get; set; } = 0;

        public int TotalCompletedItems { get; set; } = 0;

        public int TotalUncompletedItems { get; set; } = 0; 
    }
}
