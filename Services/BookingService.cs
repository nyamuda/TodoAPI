using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Dtos.Booking;
namespace TodoAPI.Services
{
    public class BookingService
    {

        private readonly ApplicationDbContext _context;
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;


        public BookingService(ApplicationDbContext context, TemplateService templateService, EmailSender emailSender)
        {
            _context = context;
            _templateService = templateService;
            _emailSender = emailSender;
        }

        // Add a new booking
        public async Task<Booking> AddBooking(AddBookingDto bookingDto, string email)
        {
            //get the user with the given email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //get the service type
            var serviceType= await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if(serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            var booking = new Booking
            {
                VehicleType = bookingDto.VehicleType,
                ServiceType = serviceType,
                Location=bookingDto.Location,
                ScheduledAt = bookingDto.ScheduledAt,
                AdditionalNotes = bookingDto.AdditionalNotes,
                User = user,
            };
            // Add a new booking to the database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            //send an email to notify the user of the new booking
            var name = user.Name;
            var phone = user.Phone;
            var location = bookingDto.Location;
            var vehicleType = bookingDto.VehicleType;
            var scheduleAt = bookingDto.ScheduledAt;
            var additionalNotes = bookingDto.AdditionalNotes;

            var emailBody = _templateService.BookingCreated(name, email, phone, serviceType, vehicleType, location, scheduleAt, additionalNotes);
            var emailSubject = "New Booking Created";
            await _emailSender.SendEmail(name, email, emailSubject, emailBody);

            //send an email to notify the admin of the new booking
            var adminEmail = _emailSender.AdminEmail;
            await _emailSender.SendEmail(name:"Admin", email:adminEmail, subject:emailSubject, message:emailBody);

            return booking;

        }
        // Add guest booking when user is not logged in and wants to create a booking
        public async Task<Booking> AddGuestBooking(AddGuestBookingDto bookingDto)
        {
            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            var booking = new Booking
            {
                GuestName = bookingDto.GuestName,
                GuestEmail = bookingDto.GuestEmail,
                GuestPhone = bookingDto.GuestPhone,
                Location=bookingDto.Location,
                VehicleType = bookingDto.VehicleType,
                ServiceType = serviceType,
                ScheduledAt = bookingDto.ScheduledAt,
                AdditionalNotes = bookingDto.AdditionalNotes
            };
            // Add the new booking to the database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            //send an email to notify the user of the new booking
            var name = booking.GuestName;
            var phone = booking.GuestPhone;
            var email = booking.GuestEmail;
            var location = booking.Location;
            var vehicleType = booking.VehicleType;
            var scheduleAt = booking.ScheduledAt;
            var additionalNotes = booking.AdditionalNotes;

            var emailBody = _templateService.BookingCreated(name, email, phone, serviceType, vehicleType, location, scheduleAt, additionalNotes);
            var emailSubject = "New Booking Created";
            await _emailSender.SendEmail(name, email, emailSubject, emailBody);

            //send an email to notify the admin of the new booking
            var adminEmail = _emailSender.AdminEmail;
            await _emailSender.SendEmail(name: "Admin", email: adminEmail, subject: emailSubject, message: emailBody);

            return booking;
        }

        //Update an booking
        //so far there is only one field that needs to be updated by user ---> status
        public async Task UpdateBooking(int id, UpdateBookingDto bookingDto)
        {
            //get the booking with the given id
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
            if (booking is null)
                throw new KeyNotFoundException("Booking with the given ID does not exist.");

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            booking.Status = bookingDto.Status;

            await _context.SaveChangesAsync();

            //Send an email to notify the admin of the status change
            //For now, a user can only update the status to cancelled
            //Get the user info
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(booking.UserId));
            if (user is null)
                throw new KeyNotFoundException("The user of the booking was not found.");

            var name = user.Name;
            var email =user.Email;
            var phone = user.Phone;
            var location = bookingDto.Location;
            var vehicleType = bookingDto.VehicleType;
            
            var scheduledAt = bookingDto.ScheduledAt;
            var additionalNotes = bookingDto.AdditionalNotes;

            //if status is cancelled
            //send an email to the admin to tell them that a user booking has cancelled their booking
            if (bookingDto.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
            {
                //first, save the reason for cancelling the booking to the database
                if (string.IsNullOrWhiteSpace(bookingDto.CancelReason))
                    throw new InvalidOperationException("The reason for cancelling the booking was not provided.");

                booking.CancelReason = bookingDto.CancelReason;
                await _context.SaveChangesAsync();


                //and then send an email to the admin
                var emailBody = _templateService.BookingCancellation(name, email, phone, serviceType, vehicleType, location, scheduledAt, bookingDto.CancelReason);
                var emailSubject = "Booking Cancelled";
                var adminEmail = _emailSender.AdminEmail;
                await _emailSender.SendEmail(name:"Admin", email:adminEmail, subject:emailSubject, message:emailBody);
            }


        }

        //Get all bookings for a user
        //return a list of bookings and a PageInfo object
        public async Task<(List<Booking>, PageInfo)>  GetBookings(int page, int pageSize,User user)
        {
            var bookings= await _context.Bookings
                .Where(x => x.UserId.Equals(user.Id))
                .OrderByDescending(x =>  x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            }; 

            return (bookings, pageInfo);

        }
        //Get all completed bookings for a user  
        public async Task<(List<Booking>, PageInfo)> GetCompletedBookings(int page, int pageSize,User user)
        {
           
            var bookings = await _context.Bookings.Where(x => x.Status.Equals("completed"))
                .Where(x => x.UserId.Equals(user.Id))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that have been completed
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Equals("completed")).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (bookings, pageInfo);
        }

        //Get all pending bookings for a user
        public async Task<(List<Booking>, PageInfo)> GetPendingBookings(int page, int pageSize, User user)
        {
            var bookings = await _context.Bookings.Where(x => x.Status.Equals("pending"))
                .Where(x => x.UserId.Equals(user.Id))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that are pending
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Equals("pending")).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };

            return (bookings, pageInfo);
        }

        //Get all cancelled bookings
        public async Task<(List<Booking>,PageInfo)> GetCancelledBookings(int page, int pageSize, User user)
        {
            var bookings = await _context.Bookings.Where(x => x.Status.Equals("cancelled"))
                .Where(x => x.UserId.Equals(user.Id))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that are cancelled
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Equals("cancelled")).CountAsync();
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
            var booking= await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} does not exist.");

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

        //Get user statistics about bookings 
        public async Task<BookingUserStatsDto> GetBookingUserStatistics(string email)
        {
            //get the user info first
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //then get statistics about that user
            int totalBookings = await _context.Bookings.Where(x => x.UserId == user.Id).CountAsync();
            int totalCompletedBookings = await _context.Bookings.Where(x => x.Status.Equals("completed") && x.UserId==user.Id).CountAsync();
            int totalPendingBookings = await _context.Bookings.Where(x => x.Status.Equals("pending") && x.UserId == user.Id).CountAsync();
            int totalCancelledBookings = await _context.Bookings.Where(x => x.Status.Equals("cancelled") && x.UserId == user.Id).CountAsync();

            var userStats = new BookingUserStatsDto
            {
                TotalBookings = totalBookings,
                TotalCompletedBookings = totalCompletedBookings,
                TotalPendingBookings = totalPendingBookings,
                TotalCancelledBookings=totalCancelledBookings
            };
            return userStats;
        }


        // Add a new booking service type
        public async Task AddServiceType(ServiceTypeDto serviceTypeDto)
        {

            var serviceType = new ServiceType()
            {
                Name = serviceTypeDto.Name,
                Price = serviceTypeDto.Price
            };

            _context.Add(serviceType);
            await _context.SaveChangesAsync();



        }
        //Update booking booking service type
        public async Task UpdateServiceType(int id, ServiceTypeDto serviceTypeDto)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            serviceType.Name = serviceTypeDto.Name;
            serviceType.Price = serviceTypeDto.Price;

            await _context.SaveChangesAsync();
        }

        //Delete booking booking service type
        public async Task DeleteServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            _context.Remove(serviceType);
            await _context.SaveChangesAsync();

        }

        //Get all booking booking service types
        public async Task<List<ServiceType>> GetServiceTypes()
        {
            var serviceTypes = await _context.ServiceTypes.ToListAsync();
            return serviceTypes;
        }

        //Get booking booking service type by ID
        public async Task<ServiceType> GetServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            return serviceType;
           
        }


    }
}
