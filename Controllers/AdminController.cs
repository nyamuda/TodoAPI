using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos.Item;
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

        public AdminController(AdminService adminService, JwtService jwtService, UserService userService)
        {
            _adminService = adminService;
            _jwtService = jwtService;
            _userService = userService;
        }


        // GET: api/<ItemsController>/items
        [HttpGet("items")]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            try
            {
                var (items, pageInfo) = await _adminService.GetItems(page, pageSize);

                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        // GET: api/<ItemsController>/items/completed
        [HttpGet("items/completed")]
        public async Task<IActionResult> GetCompleted(int page = 1, int pageSize = 10)
        {
            try
            {           
             
                var (items, pageInfo) = await _adminService.GetCompletedItems(page, pageSize);

                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        // GET: api/<ItemsController>/items/pending
        [HttpGet("items/pending")]
        public async Task<IActionResult> GetPending(int page = 1, int pageSize = 10)
        {
            try
            {

                
                var (items, pageInfo) = await _adminService.GetPendingItems(page, pageSize);
                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
        // GET: api/<ItemsController>/statistics
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
                return StatusCode(500, ex.Message);
            }



        }


        // GET api/<ItemsController>/items/5
        [HttpGet("items/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
               
                var item = await _adminService.GetItem(id);
         
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        // POST api/<ItemsController>/items
        [HttpPost("items")]
        public async Task<IActionResult> Post(AddItemDto itemDto)
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

                await _adminService.AddItem(itemDto, email);

                return Created("Get", itemDto);

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

       

        // PUT api/<ItemsController>/items/5
        [HttpPut("items/{id}")]
        public async Task<IActionResult> Put(int id, UpdateItemDto itemDto)
        {
            try
            {
                await _adminService.UpdateItem(id, itemDto);
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

        // DELETE api/<ItemsController>/items/5
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                await _adminService.DeleteItem(id);
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
        //Add a car wash service
        // POST api/<ItemsController>/items/service
        [HttpPost("items/service")]
        public async Task<IActionResult> AddServiceType(ServiceTypeDto serviceTypeDto)
        {
            try
            {
                await _adminService.AddServiceType(serviceTypeDto);
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
    }
}
