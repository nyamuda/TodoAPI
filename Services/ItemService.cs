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
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;


        public ItemService(ApplicationDbContext context, TemplateService templateService, EmailSender emailSender)
        {
            _context = context;
            _templateService = templateService;
            _emailSender = emailSender;
        }

        // Add a new item
        public async Task AddItem(AddItemDto itemDto, string email)
        {
            //get the user with the given email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            var item = new Item
            {
                VehicleType = itemDto.VehicleType,
                ServiceType = itemDto.ServiceType,
                Location=itemDto.Location,
                ScheduledAt = itemDto.ScheduledAt,
                AdditionalNotes = itemDto.AdditionalNotes,
                User = user,
            };
            // Add a new item to the database
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            //send an email to notify the user of the new booking
            var name = user.Name;
            var phone = user.Phone;
            var location = itemDto.Location;
            var vehicleType = itemDto.VehicleType;
            var serviceType = itemDto.ServiceType;
            var scheduleAt = itemDto.ScheduledAt;
            var additionalNotes = itemDto.AdditionalNotes;

            var emailBody = _templateService.BookingCreated(name, email, phone, serviceType, vehicleType, location, scheduleAt, additionalNotes);
            var emailSubject = "New Booking Created";
            await _emailSender.SendEmail(name, email, emailSubject, emailBody);

            //send an email to notify the admin of the new booking
            var adminEmail = _emailSender.AdminEmail;
            await _emailSender.SendEmail(name:"Admin", email:adminEmail, subject:emailSubject, message:emailBody);

        }
        // Add guest item when user is not logged in and wants to create a booking
        public async Task AddGuestItem(AddGuestItemDto itemDto)
        {
            var item = new Item
            {
                GuestName = itemDto.GuestName,
                GuestEmail = itemDto.GuestEmail,
                GuestPhone = itemDto.GuestPhone,
                VehicleType = itemDto.VehicleType,
                ServiceType = itemDto.ServiceType,
                ScheduledAt = itemDto.ScheduledAt,
                AdditionalNotes = itemDto.AdditionalNotes
            };
            // Add a new item to the database
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            //send an email to notify the user of the new booking
            var name = item.GuestName;
            var phone = item.GuestPhone;
            var email = item.GuestEmail;
            var location = item.GuestLocation;
            var vehicleType = item.VehicleType;
            var serviceType = item.ServiceType;
            var scheduleAt = item.ScheduledAt;
            var additionalNotes = item.AdditionalNotes;

            var emailBody = _templateService.BookingCreated(name, email, phone, serviceType, vehicleType, location, scheduleAt, additionalNotes);
            var emailSubject = "New Booking Created";
            await _emailSender.SendEmail(name, email, emailSubject, emailBody);

            //send an email to notify the admin of the new booking
            var adminEmail = _emailSender.AdminEmail;
            await _emailSender.SendEmail(name: "Admin", email: adminEmail, subject: emailSubject, message: emailBody);
        }

        //Update an item
        //so far there is only one field that needs to be updated --- isCompleted
        public async Task UpdateItem(int id, UpdateItemDto itemDto)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item is null)
                throw new KeyNotFoundException("Item with the given ID does not exist.");

            item.Status = itemDto.Status;

            await _context.SaveChangesAsync();
        }

        //Get all items
        //return a list of items and a PageInfo object
        public async Task<(List<Item>, PageInfo)>  GetItems(int page, int pageSize,User user)
        {
            var items= await _context.Items
                .Where(x => x.UserId.Equals(user.Id))
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
        public async Task<(List<Item>, PageInfo)> GetCompletedItems(int page, int pageSize,User user)
        {
           
            var items = await _context.Items.Where(x => x.Status.Equals("completed",StringComparison.OrdinalIgnoreCase))
                .Where(x => x.UserId.Equals(user.Id))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalItems = await _context.Items.Where(x => x.Status.Equals("completed",StringComparison.OrdinalIgnoreCase)).CountAsync();
            bool hasMore = totalItems > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (items, pageInfo);
        }

        //Get all items that have not been completed
        public async Task<(List<Item>, PageInfo)> GetPendingItems(int page, int pageSize, User user)
        {
            var items = await _context.Items.Where(x => x.Status.Equals("pending",StringComparison.OrdinalIgnoreCase))
                .Where(x => x.UserId.Equals(user.Id))
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
            var item= await _context.Items.FirstOrDefaultAsync(x => x.Id == id);

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

        //Get user statistics about items 
        public async Task<ItemUserStatsDto> GetItemUserStatistics(string email)
        {
            //get the user info first
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //then get statistics about that user
            int totalItems = await _context.Items.Where(x => x.UserId == user.Id).CountAsync();
            int totalCompletedItems = await _context.Items.Where(x => x.Status.Equals("completed",StringComparison.OrdinalIgnoreCase) && x.UserId==user.Id).CountAsync();
            int totalPendingItems = await _context.Items.Where(x => x.Status.Equals("pending",StringComparison.OrdinalIgnoreCase) && x.UserId == user.Id).CountAsync();
            int totalCancelledItems = await _context.Items.Where(x => x.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase) && x.UserId == user.Id).CountAsync();

            var userStats = new ItemUserStatsDto
            {
                TotalItems = totalItems,
                TotalCompletedItems = totalCompletedItems,
                TotalPendingItems = totalPendingItems,
                TotalCancelledItems=totalCancelledItems
            };
            return userStats;
        }
      


    }
}
