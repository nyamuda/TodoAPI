﻿using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Dtos.User;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Dtos.Booking;
using TodoAPI.Dtos.CancelDetails;
namespace TodoAPI.Services
{
    public class BookingService
    {

        private readonly ApplicationDbContext _context;
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;
        private readonly StatusService _statusService;
        private readonly UserService _userService;


        public BookingService(ApplicationDbContext context, TemplateService templateService, EmailSender emailSender, StatusService statusService, UserService userService)
        {
            _context = context;
            _templateService = templateService;
            _emailSender = emailSender;
            _statusService = statusService;
            _userService = userService;
        }

        // Add a new booking
        public async Task<BookingDto> AddBooking(AddBookingDto bookingDto, string email)
        {
            //get the user with the given email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            //default booking status is "pending"
            //get status with name "pending"
            //and add it to the booking
            var status = await _context.Statuses.FirstOrDefaultAsync(x => x.Name.Equals("pending"));

            //if status with name "pending" does not exist,
            //create one
            if (status is null)
            {
                status = new Status
                {
                    Name = "pending"
                };
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();
            }

            var booking = new Booking
            {
                VehicleType = bookingDto.VehicleType,
                ServiceTypeId = serviceType.Id,
                Location = bookingDto.Location,
                ScheduledAt = bookingDto.ScheduledAt,
                AdditionalNotes = bookingDto.AdditionalNotes,
                User = user,
                Status = status
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
            await _emailSender.SendEmail(name: "Admin", email: adminEmail, subject: emailSubject, message: emailBody);

            //map Booking to BoookingDto
            return BookingDto.MapFrom(booking);

        }
        // Add guest booking when user is not logged in and wants to create a booking
        public async Task<Booking> AddGuestBooking(AddGuestBookingDto bookingDto)
        {
            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            //default booking status is "pending"
            //get status with name "pending"
            //and add it to the booking
            var status = await _context.Statuses.FirstOrDefaultAsync(x => x.Name.Equals("pending"));

            //if status with name "pending" does not exist,
            //create one
            if (status is null)
            {
                status = new Status
                {
                    Name = "pending"
                };
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();
            }

            //create the booking
            GuestUser guestUser = new GuestUser()
            {
                Name = bookingDto.GuestName,
                Email = bookingDto.GuestEmail,
                Phone = bookingDto.GuestPhone
            };
            var booking = new Booking
            {
                GuestUser = guestUser,
                Location = bookingDto.Location,
                VehicleType = bookingDto.VehicleType,
                ServiceTypeId = serviceType.Id,
                ScheduledAt = bookingDto.ScheduledAt,
                AdditionalNotes = bookingDto.AdditionalNotes,
                Status = status
            };
            // Add the new booking to the database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            //send an email to notify the user of the new booking
            var name = booking.GuestUser.Name;
            var phone = booking.GuestUser.Phone;
            var email = booking.GuestUser.Email;
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
        public async Task UpdateBooking(int id, UpdateBookingDto bookingDto, User user)
        {
            //get the booking with the given id
            var booking = await GetBooking(id);

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");

            //To ensure proper handling of bookings,
            //users are only allowed to update their booking details
            //while the booking status is "Pending." 
            //Once a booking is "Confirmed" or "En Route,"
            //users cannot update their bookings but they can still cancel.
            if (!booking.Status.Name.Equals("pending"))
                throw new InvalidOperationException("Bookings can only be updated while in the 'Pending' status.");



            booking.ServiceType = serviceType;
            booking.VehicleType = bookingDto.VehicleType;
            booking.Location = bookingDto.Location;
            booking.ScheduledAt = bookingDto.ScheduledAt;
            booking.AdditionalNotes = bookingDto.AdditionalNotes;

            await _context.SaveChangesAsync();


        }

        //Get all bookings for a user
        //return a list of bookings and a PageInfo object
        public async Task<(List<BookingDto>, PageInfo)> GetBookings(int page, int pageSize, User user, string status)
        {
            var query = _context.Bookings.AsQueryable();

            // Apply filter only if "status" is not null and not "all"
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(x => x.Status.Name.Equals(status));
            }

            var bookings = await query
                .Where(x => x.UserId.Equals(user.Id))
                .Include(x => x.ServiceType)
                .Include(x => x.Status)
                .Include(x => x.Feedback)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings
            var totalBookings = await query.Where(x => x.UserId.Equals(user.Id)).CountAsync();
            bool hasMore = totalBookings > page * pageSize;

            var pageInfo = new PageInfo()
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore

            };
            //map Booking to BoookingDto
            var bookingDtos = bookings.Select(b => BookingDto.MapFrom(b)).ToList();
            
            return (bookingDtos, pageInfo);

        }



        //Get an booking by id
        public async Task<BookingDto> GetBooking(int id)
        {
           
            var booking = await _context.Bookings.Include(x => x.Feedback)
                .Include(b => b.ServiceType)
                .Include(b => b.User)
                .Include(b => b.Status)
                .Include(b => b.CancelDetails)
                .ThenInclude(cd => cd!.CancelledByUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} does not exist.");

            return BookingDto.MapFrom(booking);
        }

        //Delete an booking
        public async Task DeleteBooking(int id)
        {

            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} does not exist.");

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
            int totalCompletedBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("completed") && x.UserId == user.Id).CountAsync();
            int totalPendingBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("pending") && x.UserId == user.Id).CountAsync();
            int totalCancelledBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("cancelled") && x.UserId == user.Id).CountAsync();
            int totalConfirmedBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("confirmed") && x.UserId == user.Id).CountAsync();

            var userStats = new BookingUserStatsDto
            {
                TotalBookings = totalBookings,
                TotalCompletedBookings = totalCompletedBookings,
                TotalPendingBookings = totalPendingBookings,
                TotalCancelledBookings = totalCancelledBookings,
                TotalConfirmedBookings = totalConfirmedBookings
            };
            return userStats;
        }

        //Change the status of a booking
        public async Task ChangeBookingStatus(BookingDto bookingDto, User user, BookingStatusUpdateDto statusUpdateDto)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id.Equals(bookingDto.Id));

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {bookingDto.Id} does not exist.");

            //get the status with the given ID
            Status status = await _statusService.GetStatusByName(statusUpdateDto.StatusName);

            //check to see if the new status of the booking is different from the old one
            if (booking.StatusId.Equals(status.Id))
                throw new InvalidOperationException($"The booking already has the status \"{status.Name}\".");

            //Normal users can only change status to "cancelled"
            //They can only cancel their bookings
            if (user.Role.Equals("User"))
            {
                if (!status.Name.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("You're only authorized to change the status to \"cancelled\".");
            }
            //if status is changed to "cancelled", then a reason must be provided
            //check if the reason for cancelling the booking was provided
            if (string.IsNullOrWhiteSpace(statusUpdateDto.CancelReason) && status.Name.Equals("cancelled"))
                throw new InvalidOperationException("The reason for cancelling the booking was not provided.");

            //if the booking is cancelled
            //provide the reason and the user who cancelled the booking
            booking.Status = status;
            if (status.Name.Equals("cancelled"))
            {

                //cancel details
                var cancelDetails = new CancelDetails()
                {
                    CancelReason = statusUpdateDto.CancelReason!,
                    CancelledByUserId = user.Id

                };
                booking.CancelDetails = cancelDetails;

            }

            _context.Update(booking);
            await _context.SaveChangesAsync();

            //send email to an admin
            //to let them known that the status of the booking has be changed by the user
            await EmailAboutStatusChange(booking);


        }

        //Send an email to an admin about a change in status of a booking
        //to tell them that a user has cancelled their booking
        private async Task EmailAboutStatusChange(Booking booking)
        {
            var (name, email, phone) = await GetBookingUserInfo(booking.Id);


            var location = booking.Location;
            var vehicleType = booking.VehicleType;
            var serviceType = booking.ServiceType;
            var scheduledAt = booking.ScheduledAt;
            var additionalNotes = booking.AdditionalNotes;
            var cancelReason = !string.IsNullOrEmpty(booking.CancelDetails?.CancelReason) ? booking.CancelDetails.CancelReason : "";


            //email body and subject
            var emailBody = string.Empty;
            var emailSubject = string.Empty;

            if (booking.Status.Name.Equals("cancelled", StringComparison.OrdinalIgnoreCase))
            {

                //send an email to the admin
                emailBody = _templateService.BookingCancellation(name, email, phone, serviceType, vehicleType, location, scheduledAt, cancelReason);
                emailSubject = "Booking Cancelled";
                var adminEmail = _emailSender.AdminEmail;
                await _emailSender.SendEmail(name: "Admin", email: adminEmail, subject: emailSubject, message: emailBody);
            }

        }

        //Get information about a user who made a specific booking
        public async Task<(string name, string email, string phone)> GetBookingUserInfo(int id)
        {

            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} does not exist.");


            var name = string.Empty;
            var email = string.Empty;
            var phone = string.Empty;

            // if the user who made the booking was a guest user
            if (booking.UserId is null)
            {
                if (booking.GuestUser is null)
                    throw new InvalidOperationException("Booking lacks key user information such as name, email and phone.");

                name = booking.GuestUser.Name;
                email = booking.GuestUser.Email;
                phone = booking.GuestUser.Phone;

            }

            else
            {
                if (booking.User is null)
                {
                    int userId = (int)booking.UserId;
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
                    if (user is null)
                        throw new KeyNotFoundException($"User with ID {id} was not found.");

                    name = user.Name;
                    email = user.Email;
                    phone = user.Phone;
                }
                else
                {
                    name = booking.User.Name;
                    email = booking.User.Email;
                    phone = booking.User.Phone;
                }

            }
            return (name, email, phone);
        }


    }
}
