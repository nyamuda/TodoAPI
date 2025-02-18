using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos.Booking;
using TodoAPI.Models;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;
        private readonly BookingService _bookingService;


        public FeedbackController(FeedbackService feedbackService, JwtService jwtService, UserService userService, BookingService bookingService)
        {
            _feedbackService = feedbackService;
            _jwtService = jwtService;
            _userService = userService;
            _bookingService = bookingService;
        }
        // GET: api/<FeedbackController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var feedback = await _feedbackService.GetAllFeedback();
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }

        // GET api/<FeedbackController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {

                var feedback = await _feedbackService.GetFeedbackById(id);
                return Ok(feedback);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });

            }
        }

        // POST api/<FeedbackController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(BookingFeedbackDto feedbackDto)
        {
            try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email
                var tokenEmail = claims.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(tokenEmail))
                {
                    throw new UnauthorizedAccessException("Access denied. The token lacks necessary claims for verification.");
                }

                //Feedback for a car wash service can come from both registered and unregistered users
                //For a user to provide feedback when the booking is completed,
                //their email (from the token) must match the email of the user who made the booking (the booking user email)
                //In other words, users must only give feedback to the booking they created themselves
              
                //First, get the booking they're trying to give feedback for
                var booking = await _bookingService.GetBooking(feedbackDto.BookingId);

                //Second, get the user email from the booking
                //Depending on whether they're a registered user or a guest(unregister user)
                string? bookedUserEmail = booking.User is not null ? booking.User.Email : booking.GuestUser?.Email;

                if (bookedUserEmail == null)
                    throw new InvalidOperationException("Booking details are incomplete. Unable to verify the user.");

                //if the emails don't match,
                //then the user is not authorized to provide feedback for this booking
                //since they are not the one who created the booking
                if (!tokenEmail.Equals(bookedUserEmail))
                    throw new UnauthorizedAccessException("You can only provide feedback for your own booking.");


                //Add the feedback
              var feedback=  await _feedbackService.AddFeedback(feedbackDto);

                return CreatedAtAction(nameof(Get),new {id=feedback.Id},feedback);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // PUT api/<FeedbackController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, BookingFeedbackDto feedbackDto)
        {
            try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email
                var email = claims.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new UnauthorizedAccessException("Access denied. The token lacks necessary claims for verification.");
                }
                //get user with the given email
                var user = await _userService.GetUserByEmail(email);
                //booking they're trying to access
                var booking = await _bookingService.GetBooking(feedbackDto.BookingId);

                //for a user to perform this request, their ID 
                //must match the UserId of the booking
                //In other words, users must only update feedback of the booking they created themselves
                if (!booking.UserId.Equals(user.Id))
                {
                    throw new UnauthorizedAccessException("Access denied. You're not authorized to update this feedback.");
                }

                //update the feedback
                await _feedbackService.UpdateFeedback(id, feedbackDto);

                return NoContent();
              
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE api/<FeedbackController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
           try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email
                var email = claims.FindFirst(ClaimTypes.Email)?.Value;


                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new UnauthorizedAccessException("Access denied. The token lacks necessary claims for verification.");
                }
                //get user with the given email
                var user = await _userService.GetUserByEmail(email);

                //booking they're trying to access
                var feedback = await _feedbackService.GetFeedbackById(id);
                //the booking the feedback is for
                var booking = await _bookingService.GetBooking(feedback.BookingId);

                //for a user to perform this request, their ID 
                //must match the UserId of the booking
                //In other words, users must only delete feedback they created themselves OR
                //if they're the admin
                if (!booking.UserId.Equals(user.Id) && !user.Role.Equals("Admin"))
                {
                    throw new UnauthorizedAccessException("Access denied. You're not authorized to delete this feedback.");
                }

                //delete feedback
                await _feedbackService.DeleteFeedback(id);

                return NoContent();
            }


            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
