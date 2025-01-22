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

        public ItemsController(ItemService itemService, JwtService jwtService)
        {
            _itemService = itemService;
            _jwtService = jwtService;
        }

        // GET: api/<ItemsController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            //First, get the access token for the authorized user
            // Get the token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ///validate and decode the token
            ClaimsPrincipal claims = _jwtService.ValidateToken(token);

            //get the email
            var email = claims.FindFirst(ClaimTypes.Email)?.Value;

            if (email is null)
                return NotFound(new { message = "Email field not found from the provided token." });

            var (items, pageInfo) = await _itemService.GetItems(page, pageSize, email);

            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }


        // GET: api/<ItemsController>/completed
        [HttpGet("completed")]
        [Authorize]
        public async Task<IActionResult> GetCompleted(int page = 1, int pageSize = 10)
        {
            //First, get the access token for the authorized user
            // Get the token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ///validate and decode the token
            ClaimsPrincipal claims = _jwtService.ValidateToken(token);

            //get the email
            var email = claims.FindFirst(ClaimTypes.Email)?.Value;

            if (email is null)
                return NotFound(new { message = "Email field not found from the provided token." });

            //get the items of a user with that email
            var (items, pageInfo) = await _itemService.GetCompletedItems(page, pageSize,email);

            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }

        // GET: api/<ItemsController>/uncompleted
        [HttpGet("pending")]
        [Authorize]
        public async Task<IActionResult> GetUncompleted(int page = 1, int pageSize = 10)
        {
            //First, get the access token for the authorized user
            // Get the token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            ///validate and decode the token
            ClaimsPrincipal claims = _jwtService.ValidateToken(token);

            //get the email
            var email = claims.FindFirst(ClaimTypes.Email)?.Value;

            if (email is null)
                return NotFound(new { message = "Email field not found from the provided token." });

            //get the items of a user with that email
            var (items, pageInfo) = await _itemService.GetPendingItems(page, pageSize,email);
            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }
        //User statistics such as the number of completed items
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
                    throw new KeyNotFoundException("Email field not found from the provided token.");


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
            var item = await _itemService.GetItem(id);
            if (item == null)
            {
                return NotFound(new {message="The item with the provided item ID was not found"});
            }
            return Ok(item);
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
                    throw new InvalidOperationException("Provided access token does not have an email address.");
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
        [Authorize]
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
