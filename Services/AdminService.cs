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
        private readonly StatusService _statusService;
        private readonly BookingService _bookingService;
        private readonly AppService _appService;
        private readonly JwtService _jwtService;

        public AdminService(ApplicationDbContext context, TemplateService templateService, EmailSender emailSender, StatusService statusService, BookingService bookingService, AppService appService, JwtService jwtService)
        {
            _context = context;
            _templateService = templateService;
            _emailSender = emailSender;
            _statusService = statusService;
            _bookingService = bookingService;
            _appService = appService;
            _jwtService = jwtService;
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

            booking.ServiceType = serviceType;
            booking.VehicleType = bookingDto.VehicleType;
            booking.Location = bookingDto.Location;
            booking.ScheduledAt = bookingDto.ScheduledAt;
            booking.AdditionalNotes = bookingDto.AdditionalNotes;

            await _context.SaveChangesAsync();


        }

        //Get all bookings
        //return a list of bookings and a PageInfo object
        public async Task<(List<Booking>, PageInfo)> GetBookings(int page, int pageSize, string? status)
        {

            var query = _context.Bookings.AsQueryable();

            // Apply filter only if "status" is not null and not "all"
            if (!string.IsNullOrEmpty(status) && !status.Equals("all"))
            {
                query = query.Where(x => x.Status.Name == status);
            };

            var bookings = await query
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.ServiceType)
                .Include(x => x.Status)
                .Include(x => x.User)
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



        //Get an booking by id
        public async Task<Booking> GetBooking(int id)
        {
            var booking = await _context.Bookings.Include(b => b.ServiceType).Include(b => b.User).FirstOrDefaultAsync(x => x.Id == id);

            if (booking is null)
                throw new KeyNotFoundException("Booking with the given ID does not exist.");

            return booking;
        }

        //Delete an booking
        public async Task DeleteBooking(int id)
        {
            var booking = await GetBooking(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        //Get statistics about bookings 
        public async Task<BookingAdminStatsDto> GetStatistics()
        {


            //then get statistics for the admin
            int totalBookings = await _context.Bookings.CountAsync();
            int totalCompletedBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("completed")).CountAsync();
            int totalPendingBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("pending")).CountAsync();
            int totalCancelledBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("cancelled")).CountAsync();
            int totalConfirmedBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("confirmed")).CountAsync();
            int totalEnRouteBookings = await _context.Bookings.Where(x => x.Status.Name.Equals("en route")).CountAsync();
            //total clients registered excluding Admin
            int totalRegisteredUsers = await _context.Users.Where(x => x.Role != "Admin").CountAsync();


            var adminStats = new BookingAdminStatsDto
            {
                TotalBookings = totalBookings,
                TotalCompletedBookings = totalCompletedBookings,
                TotalPendingBookings = totalPendingBookings,
                TotalCancelledBookings = totalCancelledBookings,
                TotalConfirmedBookings = totalConfirmedBookings,
                TotalEnRouteBookings = totalEnRouteBookings,
                TotalRegisteredUsers=totalRegisteredUsers
            };
            return adminStats;
        }

        //Change booking status
        public async Task ChangeBookingStatus(int id, User user, BookingStatusUpdateDto statusUpdateDto)
        {
            //get the booking with the given ID
            Booking booking = await _bookingService.GetBooking(id);
            //get the status with the given name
            Status status = await _statusService.GetStatusByName(statusUpdateDto.StatusName);

            //check to see if the new status of the booking is different from the old one
            if (booking.StatusId.Equals(status.Id))
                throw new InvalidOperationException($"The booking already has the status \"{status.Name}\".");

            //if status is changed to "cancelled", then a reason must be provided
            //check if the reason for cancelling the booking was provided
            if (string.IsNullOrWhiteSpace(statusUpdateDto.CancelReason) && status.Name.Equals("cancelled"))
                throw new InvalidOperationException("The reason for cancelling the booking was not provided.");

            //if the booking is cancelled
            //provide the reason and the admin who cancelled the booking
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

            //send email to the user who made the booking
            //to let them know that the status of the booking has be changed
            await EmailAboutStatusChange(booking);


        }

        //Send an email to a user and tell them about a change in the status of their booking
        private async Task EmailAboutStatusChange(Booking booking)
        {

            var (name, email, phone) = await _bookingService.GetBookingUserInfo(booking.Id);

            var location = booking.Location;
            var vehicleType = booking.VehicleType;
            var serviceType = booking.ServiceType;
            var scheduledAt = booking.ScheduledAt;
            var additionalNotes = booking.AdditionalNotes;
            var cancelReason = !string.IsNullOrEmpty(booking.CancelDetails?.CancelReason) ? booking.CancelDetails.CancelReason : "";


            //email body and subject
            var emailBody = string.Empty;
            var emailSubject = string.Empty;

            //then send an email to the user
            switch (booking.Status.Name)
            {
                case "confirmed":
                    emailBody = _templateService.BookingConfirmation(name, email, phone, serviceType, vehicleType, location, scheduledAt, additionalNotes);
                    emailSubject = "Booking Confirmed";
                    await _emailSender.SendEmail(name, email, emailSubject, emailBody);
                    break;
                case "cancelled":
                    emailBody = _templateService.BookingCancellation(name, email, phone, serviceType, vehicleType, location, scheduledAt, cancelReason);
                    emailSubject = "Booking Cancelled";
                    await _emailSender.SendEmail(name, email, emailSubject, emailBody);
                    break;
                case "en route":
                    emailBody = _templateService.BookingEnRoute(name, scheduledAt, location);
                    emailSubject = "Car Wash On the Way";
                    await _emailSender.SendEmail(name, email, emailSubject, emailBody);
                    break;
                case "completed":
                    //send email to the user to notify them that their booking has been completed
                    //and encourage them to provide feedback for the service they received

                    //Create JWT token for the feedback URL
                    bool isUserVerified = booking.User is not null ? booking.User.IsVerified : false;
                    User user = new User()
                    {
                        Name = name,
                        Email = email,
                        Role = "User",
                        IsVerified = isUserVerified
                    };
                    string token = _jwtService.GenerateJwtToken(user);
                    string feedbackUrl = $"{_appService.AppDomainName}/feedback?bookingId={booking.Id}&token={token}";
                    emailBody = _templateService.BookingCompletedEmail(feedbackUrl, name, booking.ServiceType.Name);
                    emailSubject = "Your Car Wash is Finished – Leave a Review";
                    await _emailSender.SendEmail(name, email, emailSubject, emailBody);
                    break;
                default:
                    break;

            }


        }


    }
}
