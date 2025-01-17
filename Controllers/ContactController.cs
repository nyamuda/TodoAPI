using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Contact;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {

        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }
       

        // POST api/<ContactController>
        [HttpPost]
        public async Task<IActionResult> Post(ContactUsDto contactUsDto)
        {
            try
            {
                await _contactService.SendContactUsEmail(contactUsDto);

                return StatusCode(201, new { message = "The message has been received." });
                
            }

            catch (KeyNotFoundException ex) {
                return NotFound(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
