using System.Text.Json.Serialization;
using TodoAPI.Dtos.CancelDetails;
using TodoAPI.Models;
using TodoAPI.Dtos;
using TodoAPI.Dtos.User;
namespace TodoAPI.Dtos.Booking
{
    public class BookingDto
    {

        public int Id { get; set; }
        public string VehicleType { get; set; } = default!;
        public int ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; } = default!;
        public string Location { get; set; } = default!;
        public int StatusId { get; set; }

        public Status Status { get; set; }

        public CancelDetailsDto? CancelDetails { get; set; } //details about a cancelled booking
        public DateTime ScheduledAt { get; set; }

        public string? AdditionalNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; } // Nullable for guest bookings

        public UserDto? User { get; set; } // Nullable for guest bookings

        //Guest user for users not registered
        public GuestUser? GuestUser { get; set; }


        public Feedback? Feedback { get; set; }



        public BookingDto MapFrom(Models.Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                VehicleType = booking.VehicleType,
                ServiceTypeId = booking.ServiceTypeId,
                ServiceType = booking.ServiceType,
                Location = booking.Location,
                StatusId = booking.StatusId,
                Status = booking.Status,
                ScheduledAt = booking.ScheduledAt,
                AdditionalNotes = booking.AdditionalNotes,
                CreatedAt = booking.CreatedAt,
                UserId = booking.UserId,
                User = booking.User != null ? new UserDto // Mapping user details if the user exists
                {
                    Id = (int)booking.UserId!,
                    Name = booking.User.Name,
                    Email = booking.User.Email,
                    Phone = booking.User.Phone,
                    Role = booking.User.Role,
                    IsVerified = booking.User.IsVerified
                } : null,
                GuestUser = booking.GuestUser,
                // Mapping cancellation details (if the booking was cancelled)
                CancelDetails = booking.CancelDetails != null ? new CancelDetailsDto
                {
                    CancelledAt = booking.CancelDetails.CancelledAt,
                    CancelReason = booking.CancelDetails.CancelReason,
                    CancelledByUser = new CancellingUserDTO // Mapping details of the user who cancelled the booking
                    {
                        Name = booking.CancelDetails.CancelledByUser!.Name,
                        Role = booking.CancelDetails.CancelledByUser.Role
                    }
                } : null
            };
        }



    }
}
