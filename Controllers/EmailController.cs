using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Account;
using TodoAPI.Dtos.Contact;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ContactService _contactService;
        private readonly ErrorMessageService _errorMessage;

        public EmailController(AccountService accountService, ContactService contactService, ErrorMessageService errorMessage)
        {
            _accountService = accountService;
            _contactService = contactService;
            _errorMessage = errorMessage;
        }

        // POST api/<EmailController>/-reset-password-request
        //email user who has forgotten their password
        [HttpPost("reset-password-request")]
        public async Task<IActionResult> PasswordForgot(EmailDto emailDto)
        {
            try
            {
                await _accountService.SendPasswordResetEmail(emailDto.Email);

                return Ok(new { message = "Password reset link sent successfully." });

            }
            catch (KeyNotFoundException ex)
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
        // POST api/<EmailController>/verification-request
        //Send email verification email
        [HttpPost("verification-request")]
        public async Task<IActionResult> EmailVerification(EmailDto emailDto)
        {
            try
            {
                await _accountService.SendEmailVerification(emailDto.Email);

                return Ok(new {message="Email verification link sent successfully."});

            }
            catch (KeyNotFoundException ex)
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

        // POST api/<EmailController>/contact-us
        //Send contact us email
        [HttpPost("contact-us")]
        public async Task<IActionResult> Post(ContactUsDto contactUsDto)
        {
            try
            {
                await _contactService.SendContactUsEmail(contactUsDto);

                return StatusCode(201, new { message = "The message has been received." });

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

    }
}
