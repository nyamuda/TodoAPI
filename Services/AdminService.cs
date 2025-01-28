using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Booking;
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

        // Add a new booking
        public async Task AddBooking(AddBookingDto bookingDto, string email)
        {
            //get the user with the given email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            var booking = new Booking
            {
                VehicleType = bookingDto.VehicleType,
                ServiceType = serviceType,
                Location = bookingDto.Location,
                ScheduledAt = bookingDto.ScheduledAt,
                AdditionalNotes = bookingDto.AdditionalNotes,
                User = user,
            };
            // Add a new booking to the database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

        }


        //Update the status of an booking
        public async Task UpdateBooking(int id, UpdateBookingDto bookingDto)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
            if (booking is null)
                throw new KeyNotFoundException("Booking with the given ID does not exist.");

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            booking.Status = bookingDto.Status;
            booking.ServiceType = serviceType;
            booking.VehicleType = bookingDto.VehicleType;
            booking.Location = bookingDto.Location;
            booking.ScheduledAt = bookingDto.ScheduledAt;
            booking.AdditionalNotes = bookingDto.AdditionalNotes;

            await _context.SaveChangesAsync();

            //Send an email to notify the user of the status change
            var name = "";
            var email = "";
            var phone = "";
            var location =bookingDto.Location;
            var vehicleType = bookingDto.VehicleType;
            var scheduledAt = bookingDto.ScheduledAt;
            var additionalNotes = bookingDto.AdditionalNotes;

            //Get the user who made the booking
            //check if user ID exists
            //if user ID is null, the user is a guest user
            var userId = booking.UserId;
            if(userId is null)
            {
                name = booking.GuestName;
                email = booking.GuestEmail;
                phone = booking.GuestPhone;
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
            if (bookingDto.Status.Equals("confirmed",StringComparison.OrdinalIgnoreCase))
            {

                var emailBody = _templateService.BookingConfirmation(name, email, phone, serviceType, vehicleType, location, scheduledAt, additionalNotes);
                var emailSubject = "Booking Confirmed";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }
            //if status is cancelled
            //send an email to the user to tell them that their booking has been cancelled
            if (bookingDto.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
            {
                //first, save the reason for cancelling the booking to the database
                if (string.IsNullOrWhiteSpace(bookingDto.CancelReason))
                    throw new InvalidOperationException("The reason for cancelling the booking was not provided.");
                booking.CancelReason = bookingDto.CancelReason;
                await _context.SaveChangesAsync();

                //then send an email to the user
                var emailBody = _templateService.BookingCancellation(name,email,phone,serviceType,vehicleType,location,scheduledAt,bookingDto.CancelReason);
                var emailSubject = "Booking Cancelled";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }
            //if status is en route
            //send an email to the user to tell them that the team is on their way
            if (bookingDto.Status.Equals("en route", StringComparison.OrdinalIgnoreCase))
            {
                var emailBody = _templateService.BookingEnRoute(name,scheduledAt,location);
                var emailSubject = "Car Wash On the Way";
                await _emailSender.SendEmail(name, email, emailSubject, emailBody);
            }


        }

        //Get all bookings
        //return a list of bookings and a PageInfo object
        public async Task<(List<Booking>, PageInfo)> GetBookings(int page, int pageSize)
        {
            var bookings = await _context.Bookings
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalBookings = await _context.Bookings.CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (bookings, pageInfo);

        }
        //Get all completed bookings   
        public async Task<(List<Booking>, PageInfo)> GetCompletedBookings(int page, int pageSize)
        {

            var bookings = await _context.Bookings.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalBookings = await _context.Bookings.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (bookings, pageInfo);
        }

        //Get all pending bookings
        public async Task<(List<Booking>, PageInfo)> GetPendingBookings(int page, int pageSize)
        {
            var bookings = await _context.Bookings.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalBookings = await _context.Bookings.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (bookings, pageInfo);
        }


        //Get an booking by id
        public async Task<Booking> GetBooking(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
                throw new KeyNotFoundException("Booking with the given ID does not exist.");

            return booking;
        }

        //Delete an booking
        public async Task DeleteBooking(int id)
        {

            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
            if (booking == null)
                throw new KeyNotFoundException("Booking with the given ID does not exist.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        //Get statistics about bookings 
        public async Task<BookingAdminStatsDto> GetStatistics()
        {
         

            //then get statistics for the admin
            int totalBookings = await _context.Bookings.CountAsync();
            int totalCompletedBookings = await _context.Bookings.Where(x => x.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).CountAsync();
            int totalPendingBookings = await _context.Bookings.Where(x => x.Status.Equals("pending", StringComparison.OrdinalIgnoreCase)).CountAsync();
            int totalCancelledBookings = await _context.Bookings.Where(x => x.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)).CountAsync();

            var adminStats = new BookingAdminStatsDto
            {
                TotalBookings = totalBookings,
                TotalCompletedBookings = totalCompletedBookings,
                TotalPendingBookings = totalPendingBookings,
                TotalCancelledBookings = totalCancelledBookings
            };
            return adminStats;
        }


    }
}
