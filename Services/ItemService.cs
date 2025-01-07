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

        public async Task<bool> UpdateItem(int id, UpdateItemDto itemDto)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                {
                    return false;
                }

                item.Title = itemDto.Title;
                item.Description = itemDto.Description;
                item.IsCompleted = itemDto.IsCompleted;
                item.DueDate = itemDto.DueDate;

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
        public async Task<List<Item>> GetItems()
        {
            var items= await _context.Items.ToListAsync();

            return items;
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
