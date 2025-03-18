using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos.Booking;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //all routes in this controller require authorization and the user must be an admin
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private AdminService _adminService;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;
        private readonly ErrorMessageService _errorMessage;

        public AdminController(AdminService adminService, JwtService jwtService, UserService userService, ErrorMessageService errorMessage)
        {
            _adminService = adminService;
            _jwtService = jwtService;
            _userService = userService;
            _errorMessage = errorMessage;
        }


        // GET: api/<BookingsController>/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? status = "all")
        {
            try
            {

                var (bookings, pageInfo) = await _adminService.GetBookings(page, pageSize, status?.ToLower());

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
        // GET: api/<BookingsController>/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {

                var statistics = await _adminService.GetStatistics();

                return Ok(statistics);
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


        // GET api/<BookingsController>/bookings/5
        [HttpGet("bookings/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {

                var booking = await _adminService.GetBooking(id);

                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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


        // PUT api/<BookingsController>/bookings/5
        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> Put(int id, UpdateBookingDto bookingDto)
        {
            try
            {
                await _adminService.UpdateBooking(id, bookingDto);
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

        // DELETE api/<BookingsController>/bookings/5
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                await _adminService.DeleteBooking(id);
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

        // PUT api/<BookingsController>/bookings/5/status
        //Update booking status
        [HttpPut("bookings/{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatusUpdateDto statusUpdateDto)
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

                await _adminService.ChangeBookingStatus(id, user, statusUpdateDto);
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
                });
            }
        }


    }
}
