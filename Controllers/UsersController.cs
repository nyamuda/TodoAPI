﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Services;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Account;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        private readonly ErrorMessageService _errorMessage;

        public UsersController(UserService userService, JwtService jwtService, ErrorMessageService errorMessage)
        {
            _userService = userService;
            _jwtService = jwtService;
            _errorMessage = errorMessage;
        }


        // GET: api/<BookingsController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _userService.GetUsers();

                return Ok(users);
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


        // GET api/<UsersController>/5
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

                var user = await _userService.GetUser(id);

                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} was not found." });
                }

                //for a user to perform this request, the email from their token
                //must match the email of the user they're trying to access
                //OR they should be the admin
                if (!tokenEmail.Equals(user.Email) && !role.Equals("Admin"))
                {
                    return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                }

                return Ok(user);
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

            // PUT api/<UsersController>/5
            [HttpPut("{id}")]
            [Authorize]
            public async Task<IActionResult> Put(int id, UserUpdateDto userUpdateDto)
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

                    var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                    if (tokenEmail == null || role == null)
                        return Unauthorized(new { Message = "Access denied. The token lacks necessary claims for verification." });

                    var user = await _userService.GetUser(id);

                    if (user == null)
                    {
                        return NotFound(new { Message = $"User with ID {id} was not found." });
                    }

                    //for a user to perform this request, the email from their token
                    //must match the email of the user they're trying to access
                    //OR they should be the admin
                    if (!tokenEmail.Equals(user.Email) && !role.Equals("Admin"))
                    {
                        return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                    }


                    await _userService.UpdateUser(id, userUpdateDto);

                    return NoContent();
                }

                catch (KeyNotFoundException ex)
                {
                    return NotFound(new { Message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { Message = ex.Message });
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

            // DELETE api/<UsersController>/5
            [HttpDelete("{id}")]
            [Authorize]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {//First, get the access token for the authorized user
                 // Get the token from the Authorization header
                    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    ///validate and decode the token
                    ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                    //get the email and role
                    var tokenEmail = claims.FindFirst(ClaimTypes.Email)?.Value;
                    var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                    if (tokenEmail == null || role == null)
                        return Unauthorized(new { Message = "Access denied. The token lacks necessary claims for verification." });

                    var user = await _userService.GetUser(id);

                    if (user == null)
                    {
                        return NotFound(new { Message = $"User with ID {id} was not found." });
                    }

                    //for a user to perform this request, the email from their token
                    //must match the email of the user they're trying to access
                    //OR they should be the admin
                    if (!tokenEmail.Equals(user.Email) && !role.Equals("Admin"))
                    {
                        return Unauthorized(new { Message = "Your account lacks the necessary permissions to complete this request." });
                    }

                    await _userService.DeleteUser(id);

                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(new { Message = ex.Message });
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

        // GET api/<UsersController>/me
        //Get user details using the email from their access token
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail()
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

                var user = await _userService.GetUserByEmail(tokenEmail);

                if (user == null)
                {
                    throw new KeyNotFoundException($"User with email {tokenEmail} was not found.");
                }

                return Ok(user);
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
