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
        public async Task<Booking> AddBooking(AddBookingDto bookingDto, string email)
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

            return booking;

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
        //For now, a user can only update the status to "cancelled"
        public async Task UpdateBooking(int id, UpdateBookingDto bookingDto, User user)
        {
            //get the booking with the given id
            var booking = await GetBooking(id);

            //get the service type
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == bookingDto.ServiceTypeId);

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the provided ID does not exist.");


            booking.ServiceType = serviceType;
            booking.VehicleType = bookingDto.VehicleType;
            booking.Location = bookingDto.Location;
            booking.ScheduledAt = bookingDto.ScheduledAt;
            booking.AdditionalNotes = bookingDto.AdditionalNotes;

            await _context.SaveChangesAsync();


        }

        //Get all bookings for a user
        //return a list of bookings and a PageInfo object
        public async Task<(List<Booking>, PageInfo)> GetBookings(int page, int pageSize, User user)
        {
            var bookings = await _context.Bookings
                .Where(x => x.UserId.Equals(user.Id))
                .Include(x => x.ServiceType)
                .Include(x => x.Status)
                .OrderByDescending(x => x.CreatedAt)
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
        public async Task<(List<Booking>, PageInfo)> GetCompletedBookings(int page, int pageSize, User user)
        {

            var bookings = await _context.Bookings.Where(x => x.Status.Name.Equals("completed"))
                .Where(x => x.UserId.Equals(user.Id))
                .Include(x => x.ServiceType)
                 .Include(x => x.Status)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that have been completed
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Name.Equals("completed")).CountAsync();
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
            var bookings = await _context.Bookings.Where(x => x.Status.Name.Equals("pending"))
                .Where(x => x.UserId.Equals(user.Id))
                .Include(x => x.ServiceType)
                 .Include(x => x.Status)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that are pending
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Name.Equals("pending")).CountAsync();
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
        public async Task<(List<Booking>, PageInfo)> GetCancelledBookings(int page, int pageSize, User user)
        {
            var bookings = await _context.Bookings.Where(x => x.Status.Name.Equals("cancelled"))
                .Where(x => x.UserId.Equals(user.Id))
                .Include(x => x.ServiceType)
                 .Include(x => x.Status)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total bookings that are cancelled
            var totalBookings = await _context.Bookings.Where(x => x.UserId.Equals(user.Id)).Where(x => x.Status.Name.Equals("cancelled")).CountAsync();
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
            var booking = await _context.Bookings.Include(b => b.CancelDetails).Include(b => b.Status).Include(b => b.ServiceType).Include(b => b.User).FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} does not exist.");

            return booking;
        }

        //Delete an booking
        public async Task DeleteBooking(int id)
        {

            var booking = await GetBooking(id);
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
            int totalCompletedBookings = await _context.Bookings.Where(x => x.Status.Equals("completed") && x.UserId == user.Id).CountAsync();
            int totalPendingBookings = await _context.Bookings.Where(x => x.Status.Equals("pending") && x.UserId == user.Id).CountAsync();
            int totalCancelledBookings = await _context.Bookings.Where(x => x.Status.Equals("cancelled") && x.UserId == user.Id).CountAsync();

            var userStats = new BookingUserStatsDto
            {
                TotalBookings = totalBookings,
                TotalCompletedBookings = totalCompletedBookings,
                TotalPendingBookings = totalPendingBookings,
                TotalCancelledBookings = totalCancelledBookings
            };
            return userStats;
        }

        //Change the status of a booking
        public async Task ChangeBookingStatus(Booking booking, User user, BookingStatusUpdateDto statusUpdateDto)
        {
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
                //user who cancelled the booking
                var cancelledByUser = new CancelledByUser() { Name = user.Name, Role = user.Role };
                //cancel details
                var cancelDetails = new CancelDetails()
                {
                    CancelReason = statusUpdateDto.CancelReason!,
                    CancelledByUser = cancelledByUser

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
            Booking booking = await GetBooking(id);

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
                    User user = await _userService.GetUser((int)booking.UserId);

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
