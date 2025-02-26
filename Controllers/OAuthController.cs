using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Account;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {

        private readonly FacebookService _facebookService;
        private readonly GoogleService _googleService;
        private readonly ErrorMessageService _errorMessage;

        public OAuthController(FacebookService facebookService, GoogleService googleService, ErrorMessageService errorMessage)
        {
            _facebookService = facebookService;
            _googleService = googleService;
            _errorMessage = errorMessage;
        }

        // POST api/<UsersController>/google-login
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin(CodeDto codeDto)
        {
            try
            {
                var googleAccessToken = await _googleService.GetGoogleToken(codeDto.Code);

                var googleUserDetails = await _googleService.GetGoogleUserInfo(googleAccessToken);

                //get JWT access token
                var token = _googleService.GoogleLogin(googleUserDetails);

                return Ok(new { message = "User logged in successfully.", token = token });

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




        // POST api/<OAuthController>
        [HttpPost("facebook")]
        public async Task<IActionResult> Facebook(CodeDto codeDto)
        {
            try
            {
                var facebookAccessToken = await _facebookService.GetFacebookToken(codeDto.Code);

                var facebookUserDetails = await _facebookService.GetFacebookUserInfo(facebookAccessToken);

                //get JWT access token
                var token = _facebookService.FacebookLogin(facebookUserDetails);

                return Ok(new { message = "User logged in successfully.", token = token });

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
