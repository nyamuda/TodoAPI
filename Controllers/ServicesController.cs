using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Booking;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//Car Wash Services
namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceTypesService _serviceTypesService;
        private readonly ErrorMessageService _errorMessage;


        public ServicesController(ServiceTypesService serviceTypesService, ErrorMessageService errorMessage)
        {
            _serviceTypesService = serviceTypesService;
            _errorMessage = errorMessage;
        }

        // GET: api/<ServicesController>
        [HttpGet]  
        public async Task<IActionResult> Get()
        {
            try
            {
                var serviceTypes = await _serviceTypesService.GetServiceTypes();
                return Ok(serviceTypes);
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

        // GET api/<ServicesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var serviceType = await _serviceTypesService.GetServiceType(id);
                return Ok(serviceType);
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

        // POST api/<ServicesController>
        [HttpPost]
        public async Task<IActionResult> Post(ServiceTypeDto serviceTypeDto)
        {
            try
            {
                var serviceType=await _serviceTypesService.AddServiceType(serviceTypeDto);
                return CreatedAtAction(nameof(Get), new { id = serviceType.Id }, serviceType);
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

        // PUT api/<ServicesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ServiceTypeDto serviceTypeDto)
        {
            try
            {
                await _serviceTypesService.UpdateServiceType(id, serviceTypeDto);
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

        // DELETE api/<ServicesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceTypesService.DeleteServiceType(id);
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


        // GET api/<ServicesController>/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular(int id)
        {
            try
            {
                var serviceType = await _serviceTypesService.GetPopularServiceType();
                return Ok(serviceType);
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
