using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Item;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class AdminService
    {

        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }


        //Update the status of an item
        public async Task UpdateItem(int id, UpdateItemDto itemDto)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item is null)
                throw new KeyNotFoundException("Item with the given ID does not exist.");

            item.Status = itemDto.Status;
            item.ServiceType = itemDto.ServiceType;
            item.VehicleType = itemDto.VehicleType;
            item.Location = itemDto.Location;
            item.ScheduledAt = itemDto.ScheduledAt;
            item.AdditionalNotes = itemDto.AdditionalNotes;

            await _context.SaveChangesAsync();
        }

        //Get all items
        //return a list of items and a PageInfo object
        public async Task<(List<Item>, PageInfo)> GetItems(int page, int pageSize)
        {
            var items = await _context.Items
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.CountAsync();
            bool hasMore = totalItems > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (items, pageInfo);

        }
        //Get all completed items   
        public async Task<(List<Item>, PageInfo)> GetCompletedItems(int page, int pageSize, User user)
        {

            var items = await _context.Items.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).CountAsync();
            bool hasMore = totalItems > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (items, pageInfo);
        }

        //Get all pending items
        public async Task<(List<Item>, PageInfo)> GetPendingItems(int page, int pageSize, User user)
        {
            var items = await _context.Items.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)).CountAsync();
            bool hasMore = totalItems > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (items, pageInfo);
        }


        //Get an item by id
        public async Task<Item> GetItem(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
                throw new KeyNotFoundException("Item with the given ID does not exist.");

            return item;
        }

        //Delete an item
        public async Task DeleteItem(int id)
        {

            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                throw new KeyNotFoundException("Item with the given ID does not exist.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        //Get statistics about items 
        public async Task<ItemAdminStatsDto> GetStatistics()
        {
         

            //then get statistics for the admin
            int totalItems = await _context.Items.CountAsync();
            int totalCompletedItems = await _context.Items.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).CountAsync();
            int totalPendingItems = await _context.Items.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)).CountAsync();
            int totalCancelledItems = await _context.Items.Where(x => x.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)).CountAsync();

            var adminStats = new ItemAdminStatsDto
            {
                TotalItems = totalItems,
                TotalCompletedItems = totalCompletedItems,
                TotalPendingItems = totalPendingItems,
                TotalCancelledItems = totalCancelledItems
            };
            return adminStats;
        }
    }
}
