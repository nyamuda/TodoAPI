using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Booking;
using TodoAPI.Models;
using TodoAPI.Services;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private BookingService _bookingService;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;
        private readonly ErrorMessageService _errorMessage;

        public BookingsController(BookingService bookingService, JwtService jwtService, UserService userService, ErrorMessageService errorMessage)
        {
            _bookingService = bookingService;
            _jwtService = jwtService;
            _userService = userService;
            _errorMessage = errorMessage;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10,string status="all")
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

                if (email is null)
                    return Unauthorized(new { message = "Access denied. The token lacks necessary claims for verification." });

                //get user with the email
                var user = await _userService.GetUserByEmail(email);

                if (user is null)
                    return NotFound(new { message = "User with the given email does not exist." });


                var (bookings, pageInfo) = await _bookingService.GetBookings(page, pageSize, user, status.ToLower());

                var response = new
                {
                    bookings,
                    pageInfo
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }

        }


        //User statistics such as the number of completed bookings
        // GET: api/<BookingsController>/stats
        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetBookingUserStatistics()
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

                if (email is null)
                    throw new UnauthorizedAccessException("Access denied. The token lacks necessary claims for verification.");


                //user statistics
                var statistics = await _bookingService.GetBookingUserStatistics(email);

                return Ok(statistics);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }



        }


        // GET api/<BookingsController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email and role
                var tokenEmail = claims.FindFirst(ClaimTypes.Email)?.Value;
                var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                if (tokenEmail == null || role == null)
                    return Unauthorized(new { Message = "Access denied. The token lacks necessary claims for verification." });


                //get user with the given email
                var user = await _userService.GetUserByEmail(tokenEmail);
                //booking they're trying to access
                var booking = await _bookingService.GetBooking(id);

                //for a user to perform this request, their ID 
                //must match the ID of the user of the booking they're trying to access
                //OR they should be the admin
                if (!booking.UserId.Equals(user.Id) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }



                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }


        }

        // POST api/<BookingsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(AddBookingDto addBookingDto)
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

                var booking = await _bookingService.AddBooking(addBookingDto, email);

               

                return CreatedAtAction(nameof(Get), new { id = booking.Id }, booking);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }


        }

        // POST api/<BookingsController>/guest
        [HttpPost("guest")]
        public async Task<IActionResult> PostGuestBooking(AddGuestBookingDto bookingDto)
        {
            try
            {
                Booking booking = await _bookingService.AddGuestBooking(bookingDto);
                return CreatedAtAction(nameof(Get), new { id = booking.Id }, booking);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }


        }


        // PUT api/<BookingsController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateBookingDto bookingDto)
        {
            try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email and role
                var tokenEmail = claims.FindFirst(ClaimTypes.Email)?.Value;
                var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                if (tokenEmail == null || role == null)
                    return Unauthorized(new { Message = "Access denied. The token lacks necessary claims for verification." });


                //get user with the given email
                var user = await _userService.GetUserByEmail(tokenEmail);
                //booking they're trying to access
                var booking = await _bookingService.GetBooking(id);

                //for a user to perform this request, their ID 
                //must match the ID of the user of the booking they're trying to access
                //OR they should be the admin
                if (!booking.UserId.Equals(user.Id) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }

                await _bookingService.UpdateBooking(id, bookingDto, user);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        // DELETE api/<BookingsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                await _bookingService.DeleteBooking(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }

        }

        // PUT api/<BookingsController>/5
        //Update the status of a particular booking
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id,BookingStatusUpdateDto statusUpdateDto)
        {
            try
            {
                //First, get the access token for the authorized user
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                ///validate and decode the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                //get the email and role
                var tokenEmail = claims.FindFirst(ClaimTypes.Email)?.Value;
                var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                if (tokenEmail == null || role == null)
                    throw new UnauthorizedAccessException("Access denied. The token lacks necessary claims for verification.");

                //get user with the given email
                var user = await _userService.GetUserByEmail(tokenEmail);
                //booking they're trying to access
                var booking = await _bookingService.GetBooking(id);


                //for a user to perform this request, their ID 
                //must match the ID of the user of the booking they're trying to access
                //OR they should be the admin
                if (!booking.UserId.Equals(user.Id) && !role.Equals("Admin"))
                    throw new UnauthorizedAccessException("Your account lacks the necessary permissions to complete this request.");

                await _bookingService.ChangeBookingStatus(booking,user,statusUpdateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                }); ;
            }
        }





    }
}
