using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Item;
using TodoAPI.Models;
using TodoAPI.Services;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private ItemService _itemService;
        private readonly JwtService _jwtService;
        private readonly UserService _userService;

        public ItemsController(ItemService itemService, JwtService jwtService, UserService userService)
        {
            _itemService = itemService;
            _jwtService = jwtService;
            _userService = userService;
        }

        // GET: api/<ItemsController>
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


                var (items, pageInfo) = await _itemService.GetItems(page, pageSize, user);

                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            } 
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        // GET: api/<ItemsController>/completed
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

                //get the items of a user with that email
                var (items, pageInfo) = await _itemService.GetCompletedItems(page, pageSize,user);

                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        // GET: api/<ItemsController>/pending
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

                //get the items of a user with that email
                var (items, pageInfo) = await _itemService.GetPendingItems(page, pageSize, user);
                var response = new
                {
                    items,
                    pageInfo
                };
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
        //User statistics such as the number of completed items
        // GET: api/<ItemsController>/statistics
        [HttpGet("statistics")]
        [Authorize]
        public async Task<IActionResult> GetItemUserStatistics()
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
                var statistics = await _itemService.GetItemUserStatistics(email);
                
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


        // GET api/<ItemsController>/5
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
                var user = _userService.GetUserByEmail(tokenEmail);
                //item they're trying to access
                var item = await _itemService.GetItem(id);

                //for a user to perform this request, their ID 
                //must match the ID of the user of the item they're trying to access
                //OR they should be the admin
                if (!item.UserId.Equals(user.Id) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }

                

                return Ok(item);
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

        // POST api/<ItemsController>
        [HttpPost]
        [Authorize]
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

                await _itemService.AddItem(itemDto, email);

                return Created("Get", itemDto);

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

        // POST api/<ItemsController>/guest
        [HttpPost("guest")]
        public async Task<IActionResult> PostGuestItem(AddGuestItemDto itemDto)
        {
            try
            {
                await _itemService.AddGuestItem(itemDto);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }


        // PUT api/<ItemsController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateItemDto itemDto)
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
                //item they're trying to access
                var item = await _itemService.GetItem(id);

                //for a user to perform this request, their ID 
                //must match the ID of the user of the item they're trying to access
                //OR they should be the admin
                if (!item.UserId.Equals(user.Id) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }

                await _itemService.UpdateItem(id, itemDto);
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

        // DELETE api/<ItemsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                await _itemService.DeleteItem(id);
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
    }
}
