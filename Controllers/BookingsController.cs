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

        public BookingsController(BookingService bookingService, JwtService jwtService, UserService userService)
        {
            _bookingService = bookingService;
            _jwtService = jwtService;
            _userService = userService;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
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


                var (bookings, pageInfo) = await _bookingService.GetBookings(page, pageSize, user);

                var response = new
                {
                    bookings,
                    pageInfo
                };
                return Ok(response);
            } 
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        // GET: api/<BookingsController>/completed
        [HttpGet("completed")]
        [Authorize]
        public async Task<IActionResult> GetCompleted(int page = 1, int pageSize = 10)
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

                //get the bookings of a user with that email
                var (bookings, pageInfo) = await _bookingService.GetCompletedBookings(page, pageSize,user);

                var response = new
                {
                    bookings,
                    pageInfo
                };
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        // GET: api/<BookingsController>/pending
        [HttpGet("pending")]
        [Authorize]
        public async Task<IActionResult> GetPending(int page = 1, int pageSize = 10)
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

                //get the bookings of a user with that email
                var (bookings, pageInfo) = await _bookingService.GetPendingBookings(page, pageSize, user);
                var response = new
                {
                    bookings,
                    pageInfo
                };
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { messgae = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
        // GET: api/<BookingsController>/pending
        [HttpGet("cancelled")]
        [Authorize]
        public async Task<IActionResult> GetCancelled(int page = 1, int pageSize = 10)
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

                //get the bookings of a user with that email
                var (bookings, pageInfo) = await _bookingService.GetCancelledBookings(page, pageSize, user);
                var response = new
                {
                    bookings,
                    pageInfo
                };
                return Ok(response);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { messgae = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
        //User statistics such as the number of completed bookings
        // GET: api/<BookingsController>/statistics
        [HttpGet("statistics")]
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
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
            }


        }

        // POST api/<BookingsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(AddBookingDto bookingDto)
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

                var booking=await _bookingService.AddBooking(bookingDto, email);

                return CreatedAtAction(nameof(Get), new { id = booking.Id }, booking);

            }
            catch(KeyNotFoundException ex)
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
                return StatusCode(500, ex.Message);
            }     
            
            
            }

        // POST api/<BookingsController>/guest
        [HttpPost("guest")]
        public async Task<IActionResult> PostGuestBooking(AddGuestBookingDto bookingDto)
        {
            try
            {
               Booking booking= await _bookingService.AddGuestBooking(bookingDto);
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
                return StatusCode(500, ex.Message);
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
                var user = _userService.GetUserByEmail(tokenEmail);
                //booking they're trying to access
                var booking = await _bookingService.GetBooking(id);

                //for a user to perform this request, their ID 
                //must match the ID of the user of the booking they're trying to access
                //OR they should be the admin
                if (!booking.UserId.Equals(user.Id) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }

                await _bookingService.UpdateBooking(id, bookingDto);
                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
           
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(500, new { message = ex.Message });
            }

        }
        //Add a booking service type
        // POST api/<BookingsController>/services
        [HttpPost("services")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddServiceType(ServiceTypeDto serviceTypeDto)
        {
            try
            {
                await _bookingService.AddServiceType(serviceTypeDto);
                return StatusCode(201, new { message = "Service type added successfully" });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        //Update a booking service type
        // PUT api/<BookingsController>/services/5
        [HttpPut("services/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateServiceType(int id,ServiceTypeDto serviceTypeDto)
        {
            try
            {
                await _bookingService.UpdateServiceType(id,serviceTypeDto);
                return NoContent();
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
        //Delete a booking service type
        // DELETE api/<BookingsController>/services/5
        [HttpDelete("services/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            try
            {
                await _bookingService.DeleteServiceType(id);
                return NoContent();
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
        //Get a booking service type
        //GET api/<BookingsController>/services/5
        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetServiceType(int id)
        {
            try
            {
               var serviceType= await _bookingService.GetServiceType(id);
                return Ok(serviceType);
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

        //Get all booking service types
        //GET api/<BookingsController>/services
        [HttpGet("services")]
        public async Task<IActionResult> GetServiceTypes()
        {
            try
            {
                var serviceTypes = await _bookingService.GetServiceTypes();
                return Ok(serviceTypes);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

       

    }
}
