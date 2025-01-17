using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Dtos.Item;
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
        public async Task<bool> AddItem(AddItemDto itemDto, string email)
        {
            try
            {
                //get the user with the given email
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
                if (user == null)
                    throw new KeyNotFoundException("User with the provided email not found.");

                var item = new Item
                {
                    Title = itemDto.Title,
                    Description = itemDto.Description,
                    DueDate = itemDto.DueDate,
                    User=user,
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
        public async Task<(List<Item>, PageInfo)>  GetItems(int page, int pageSize,string email)
        {
            var items= await _context.Items
                .Where(x => x.User.Email.Equals(email))
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
        public async Task<(List<Item>, PageInfo)> GetCompletedItems(int page, int pageSize,string email)
        {
            var items = await _context.Items.Where(x => x.IsCompleted == true)
                .Where(x => x.User.Email.Equals(email))
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
        public async Task<(List<Item>, PageInfo)> GetUncompletedItems(int page, int pageSize, string email)
        {
            var items = await _context.Items.Where(x => x.IsCompleted == false)
                .Where(x => x.User.Email.Equals(email))
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

        //Get user statistics about items 
        public async Task<ItemUserStatsDto> GetItemUserStatistics(string email)
        {
            //get the user info first
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new KeyNotFoundException("User with the provided email not found");

            //then get statistics about that user
            int totalItems = await _context.Items.Where(x => x.UserId == user.Id).CountAsync();
            int totalCompletedItems = await _context.Items.Where(x => x.IsCompleted == true && x.UserId==user.Id).CountAsync();
            int totalUncompletedItems = await _context.Items.Where(x => x.IsCompleted == false && x.UserId == user.Id).CountAsync();

            var userStats = new ItemUserStatsDto
            {
                TotalItems = totalItems,
                TotalCompletedItems = totalCompletedItems,
                TotalUncompletedItems = totalUncompletedItems
            };
            return userStats;
        }
      


    }
}
