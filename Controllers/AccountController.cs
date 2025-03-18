using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos;
using TodoAPI.Dtos.Account;
using TodoAPI.Services;
using TodoAPI.Models;
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
        private readonly UserService _userService;

        public AccountController(AccountService accountService, JwtService jwtService, ErrorMessageService errorMessage, UserService userService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _errorMessage= errorMessage;
            _userService = userService;
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
                    message = _errorMessage.UnexpectedErrorMessage(),
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
                var (accessToken, refreshToken) = await _accountService.Login(loginDto);

               
                //Create an HTTP-Only cookie to store the refresh token
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                    
                };

               HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                return Ok(new { message = "User logged in successfully.", token = accessToken });
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
                    message = _errorMessage.UnexpectedErrorMessage(),
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
                    message = _errorMessage.UnexpectedErrorMessage(),
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
                string email = claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field missing from token.");

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
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                foreach (var cookie in HttpContext.Request.Cookies)
                {
                    Console.WriteLine($"Cookie is {cookie.Key}: {cookie.Value}");
                }
                //Get the refresh token from the HTTP-Only cookie
                var refreshToken = HttpContext.Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    throw new UnauthorizedAccessException("Refresh token missing.");


                //validate the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(refreshToken);

                // If we got here then the token is valid
                // since there is no exception
                //get user email
                string email = claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field missing from refresh token.");

                //get the user with the given email
                User user = await _userService.GetUserByEmail(email);

                //generate the access token
                //access token lifespan is 72 hours = 4320 minutes
                var accessTokenLifespan = 4320;
                var accessToken = _jwtService.GenerateJwtToken(user: user, expiresIn: accessTokenLifespan);
                return Ok(new {token=accessToken });

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
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
