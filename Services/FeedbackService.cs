using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Booking;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class FeedbackService
    {

        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;

        }


        //Add feedback for the booking after its completed
        public async Task<Feedback> AddFeedback(BookingFeedbackDto feedbackDto, string userEmail)
        {
            // check if the booking exists
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id.Equals(feedbackDto.BookingId));

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {feedbackDto.BookingId} does not exist.");
            //check if the booking has been completed
            if (!booking.Status.Name.Equals("completed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Feedback cannot be added as the booking has not been completed.");


            //add the feedback to the database
            var feedback = new Feedback
            {
                Content = feedbackDto.Content,
                Rating = feedbackDto.Rating,
                BookingId = booking.Id,
                ServiceTypeId = booking.ServiceTypeId
            };


            //If the user is registered,
            //get their ID and add it to the feedback as well
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(userEmail));
            if (user is not null)
            {
                feedback.UserId = user.Id;
            }

            //Finally, save the feedback
            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();

            return feedback;

        }
        //Update booking feedback
        public async Task UpdateFeedback(int id, BookingFeedbackDto feedbackDto)
        {
            //check if the booking exists
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id.Equals(feedbackDto.BookingId));

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {feedbackDto.BookingId} does not exist.");


            //check if the booking has been completed
            if (!booking.Status.Name.Equals("completed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Feedback cannot be added as the booking has not been completed.");


            //check if the feedback exists
            var feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (feedback is null)
                throw new KeyNotFoundException($"Feedback with ID {id} does not exist.");

            //update the context and rating
            feedback.Content = feedbackDto.Content;
            feedback.Rating = feedbackDto.Rating;
            await _context.SaveChangesAsync();
        }

        //Get feedback by ID
        public async Task<Feedback> GetFeedbackById(int id)
        {
            //check if the feedback exists
            var feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (feedback is null)
                throw new KeyNotFoundException($"Feedback with ID {id} does not exist.");

            return feedback;
        }
        //Get all the feedback
        public async Task<(List<Feedback> feedback, PageInfo pageInfo)> GetAllFeedback(int page, int pageSize)
        {
            
            var feedback = await _context.Feedback
                .Include(x => x.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            //total feedback
            int totalFeedback = await _context.Feedback.CountAsync();
            bool hasMore = totalFeedback > page * pageSize;

          

            var pageInfo = new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore
            };

            return (feedback, pageInfo);
        }

        //Delete feedback with a given ID
        public async Task DeleteFeedback(int id)
        {
            //check if the feedback exists
            var feedback = await _context.Feedback.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (feedback is null)
                throw new KeyNotFoundException($"Feedback with ID {id} does not exist.");

            _context.Remove(feedback);

            await _context.SaveChangesAsync();

        }

        


    }
}
