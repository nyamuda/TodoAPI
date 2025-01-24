using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Item;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class AdminService
    {

        private readonly ApplicationDbContext _context;
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;

        public AdminService(ApplicationDbContext context,TemplateService templateService,EmailSender emailSender)
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

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == itemDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            var item = new Item
            {
                VehicleType = itemDto.VehicleType,
                ServiceType = serviceType,
                Location = itemDto.Location,
                ScheduledAt = itemDto.ScheduledAt,
                AdditionalNotes = itemDto.AdditionalNotes,
                User = user,
            };
            // Add a new item to the database
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

        }


        //Update the status of an item
        public async Task UpdateItem(int id, UpdateItemDto itemDto)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item is null)
                throw new KeyNotFoundException("Item with the given ID does not exist.");

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == itemDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            item.Status = itemDto.Status;
            item.ServiceType = serviceType;
            item.VehicleType = itemDto.VehicleType;
            item.Location = itemDto.Location;
            item.ScheduledAt = itemDto.ScheduledAt;
            item.AdditionalNotes = itemDto.AdditionalNotes;

            await _context.SaveChangesAsync();

            //Send an email to notify the user of the status change
            var name = "";
            var email = "";
            var phone = "";
            var location =itemDto.Location;
            var vehicleType = itemDto.VehicleType;
            var scheduledAt = itemDto.ScheduledAt;
            var additionalNotes = itemDto.AdditionalNotes;

            //Get the user who made the booking
            //check if user ID exists
            //if user ID is null, the user is a guest user
            var userId = item.UserId;
            if(userId is null)
            {
                name = item.GuestName;
                email = item.GuestEmail;
                phone = item.GuestPhone;
            }
            else
            {
                //get user
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
                if (user is null)
                    throw new KeyNotFoundException("The user of the booking was not found.");

                name = user.Name;
                email = user.Email;
                phone = user.Phone;
            }
           
            //if status is confirmed
            //send an email to the user to tell them that their booking has been confirmed
            if (itemDto.Status.Equals("confirmed",StringComparison.OrdinalIgnoreCase))
            {
                var emailBody = _templateService.BookingConfirmation(name, email, phone, serviceType, vehicleType, location, scheduledAt, additionalNotes);
                var emailSubject = "Booking Confirmed";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }
            //if status is cancelled
            //send an email to the user to tell them that their booking has been cancelled
            if (itemDto.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
            {
                var emailBody = _templateService.BookingCancellation(name,email,phone,serviceType,vehicleType,location,scheduledAt,additionalNotes);
                var emailSubject = "Booking Cancelled";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }
            //if status is en route
            //send an email to the user to tell them that the team is on their way
            if (itemDto.Status.Equals("en route", StringComparison.OrdinalIgnoreCase))
            {
                var emailBody = _templateService.BookingEnRoute(name,scheduledAt,location);
                var emailSubject = "Car Wash On the Way";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }


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
        public async Task<(List<Item>, PageInfo)> GetCompletedItems(int page, int pageSize)
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
        public async Task<(List<Item>, PageInfo)> GetPendingItems(int page, int pageSize)
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
