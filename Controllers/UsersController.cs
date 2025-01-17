using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Services;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Account;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService) {
            _userService = userService;
        }


        // GET: api/<ItemsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
           try
            {
                var users = await _userService.GetUsers();

                return Ok(users);
            }

            catch (Exception ex) { 
                return StatusCode(500, new {message=ex.Message});
            }

        }


        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUser(id);

            if(user == null)
            {
                return NotFound(new {Message= $"User with ID {id} was not found." });
            }
            return Ok(user);
        }

    
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserUpdateDto userUpdateDto)
        {
            try
            {
                await _userService.UpdateUser(id, userUpdateDto);

                return NoContent();
            }

            catch (KeyNotFoundException ex) {
                return NotFound(new {Message= ex.Message });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500,new {Message=ex.Message});
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUser(id);

                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }

        }
    }
}
