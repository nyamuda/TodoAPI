using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Account;
using TodoAPI.Services;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly JwtService _jwtService;
        private readonly ErrorMessageService _errorMessage;

        public AccountController(AccountService accountService, JwtService jwtService, ErrorMessageService errorMessage)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _errorMessage= errorMessage;
        }
       
        // POST api/<AccountController>/register
        [HttpPost("register")]
        public async Task<IActionResult> Post(UserRegisterDto registerDto)
        {
            try
            {
                await _accountService.Register(registerDto);
                
                return StatusCode(201,new { message = "User registered successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage,
                    details = ex.Message
                });
            }
        }

        // POST api/<AccountController>/login
        [HttpPost("login")]
        public async Task<IActionResult> Post(UserLoginDto loginDto)
        {
            try
            {
                var token = await _accountService.Login(loginDto);
                return Ok(new { message = "User logged in successfully.", token = token });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage,
                    details = ex.Message
                });
            }
        }


        

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset(PasswordResetDto resetDto)
        {
            try
            {
                //validate the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(resetDto.Token);

                // If we got here then the token is valid
                // since there is no exception
                //get user email
                string email=claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field not found.");

                //reset the password of the user
                await _accountService.ResetPassword(email: email, newPassword: resetDto.Password);

                return Ok();

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage,
                    details = ex.Message
                });
            }
        }
       

        //verify email by validating token
        [HttpPut("verify")]
        public async Task<IActionResult> VerifyAccount(TokenDto tokenDto)
        {
            try
            {
                //validate the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(tokenDto.Token);

                // If we got here then the token is valid
                // since there is no exception
                //get user email
                string email = claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field not found.");

                //reset the password of the user
                await _accountService.VerifyAccount(email: email);

                return NoContent();

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = _errorMessage,
                    details = ex.Message
                });
            }
        }
    }
}
