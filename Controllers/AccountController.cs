using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.Dtos;
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

        public AccountController(AccountService accountService, JwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
        }
        // GET: api/<AccountController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>/register
        [HttpPost("register")]
        public async Task<IActionResult> Post(UserRegisterDto registerDto)
        {
            try
            {
                await _accountService.Register(registerDto);
                return Ok(new {message= "User registered successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // POST api/<AccountController>/login
        [HttpPost("login")]
        public async Task<IActionResult> Post(UserLoginDto loginDto)
        {
            try
            {
               var token= await _accountService.Login(loginDto);
                return Ok(new { message = "User logged in successfully.", token=token });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // POST api/<UsersController>/google-login
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] string code)
        {
            try
            {
                var googleAccessToken = await _accountService.GetGoogleToken(code);

                var googleUserDetails = await _accountService.GetGoogleUserInfo(googleAccessToken);

                //get JWT access token
                var token = _accountService.GoogleLogin(googleUserDetails);

                return Ok(new { message = "User logged in successfully.", token = token });

            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new { message = ex.Message });
            }
        }

        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] string code)
        {
            try
            {
                var googleAccessToken = await _accountService.GetGoogleToken(code);

                var googleUserDetails = await _accountService.GetGoogleUserInfo(googleAccessToken);

                //get JWT access token
                var token = _accountService.GoogleRegister(googleUserDetails);

                return Ok(new { message = "User registered successfully.", token = token });

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

        [HttpPost("password/forgot")]
        public async Task<IActionResult> PasswordForgot([FromBody] string email)
        {
            try
            {
                await _accountService.SendPasswordResetEmail(email);

                return Ok();

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("password/reset")]
        public async Task<IActionResult> PasswordReset([FromBody] string token, [FromBody] string password)
        {
            try
            {
                //validate the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                // If we got here then the token is valid
                // since there is no exception
                //get user email
                string email=claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field not found.");

                //reset the password of the user
                await _accountService.ResetPassword(email: email, newPassword: password);

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
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //Send email verification email
        [HttpPost("/verify/email")]
        public async Task<IActionResult> EmailVerification([FromBody] string email)
        {
            try
            {
                await _accountService.SendEmailVerification(email);

                return Ok();

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //verify email by validating token
        [HttpPost("/verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] string token)
        {
            try
            {
                //validate the token
                ClaimsPrincipal claims = _jwtService.ValidateToken(token);

                // If we got here then the token is valid
                // since there is no exception
                //get user email
                string email = claims.FindFirstValue(ClaimTypes.Email) ?? throw new KeyNotFoundException("Email field not found.");

                //reset the password of the user
                await _accountService.VerifyAccount(email: email);

                return Ok();

            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
