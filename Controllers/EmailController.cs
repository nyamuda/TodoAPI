using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Account;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly AccountService _accountService;

        public EmailController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/<EmailController>/password
        //email user who has forgotten their password
        [HttpPost("password")]
        public async Task<IActionResult> PasswordForgot(EmailDto emailDto)
        {
            try
            {
                await _accountService.SendPasswordResetEmail(emailDto.Email);

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
        // POST api/<EmailController>/verify
        //Send email verification email
        [HttpPost("verify")]
        public async Task<IActionResult> EmailVerification(EmailDto emailDto)
        {
            try
            {
                await _accountService.SendEmailVerification(emailDto.Email);

                return Ok(new {message="Email sent successfully."});

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

    }
}
