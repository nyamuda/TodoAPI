using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace TodoAPI.Services
{
    public class ItemService
    {

        private readonly ApplicationDbContext _context;


        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new item
        public async Task<bool> AddItem(AddItemDto itemDto)
        {
            try
            {
                var item = new Item
                {
                    Title = itemDto.Title,
                    Description = itemDto.Description,
                    DueDate = itemDto.DueDate
                };
                // Add a new item to the database
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        //Update an item

        //Update an item
        //so far there is only one field that needs to be updated --- isCompleted
        public async Task<bool> UpdateItem(int id, UpdateItemDto itemDto)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    return false;
                }

          
                item.IsCompleted = itemDto.IsCompleted;
                

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        //Get all items
        //return a list of items and a PageInfo object
        public async Task<(List<Item>, PageInfo)>  GetItems(int page, int pageSize)
        {
            var items= await _context.Items
                .OrderByDescending(x =>  x.CreatedAt)
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
        public async Task<(List<Item>, PageInfo)> GetCompletedItems(int page, int pageSize)
        {
            var items = await _context.Items.Where(x => x.IsCompleted == true)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.Where(x => x.IsCompleted == true).CountAsync();
            bool hasMore = totalItems > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (items, pageInfo);
        }

        //Get all uncompleted items
        public async Task<(List<Item>, PageInfo)> GetUncompletedItems(int page, int pageSize)
        {
            var items = await _context.Items.Where(x => x.IsCompleted == false)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.Where(x => x.IsCompleted == false).CountAsync();
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
        public async Task<Item?> GetItem(int id)
        {
            var item= await _context.Items.FirstOrDefaultAsync(x => x.Id == id);

            return item;
        }

        //Delete an item
        public async Task<bool> DeleteItem(int id)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    return false;
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

      


    }
}
