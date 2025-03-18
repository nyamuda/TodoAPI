using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Booking;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly StatusService _statusService;
        private readonly ErrorMessageService _errorMessage;

        public StatusesController(StatusService statusService, ErrorMessageService errorMessage)
        {
            _statusService = statusService;
            _errorMessage = errorMessage;
        }
        // GET: api/<StatusesController>
        [HttpGet]
        public async Task<IActionResult> Get(string? name)
        {
            try
            {
                //check to see if the request wants to get a status by name
                //i.e if the name query parameter was provided
                //Note: status names are unique 
                //this means there can be only one status with a given name
                if (!string.IsNullOrEmpty(name))
                {
                    var status = await _statusService.GetStatusByName(name);
                    return Ok(status);
                }


                var statuses = await _statusService.GetStatuses();
                return Ok(statuses);
            }
            catch (Exception ex) {

                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        // GET api/<StatusesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var status = await _statusService.GetStatus(id);

                return Ok(status);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        // POST api/<StatusesController>
        [HttpPost]
        public async Task<IActionResult> Post(StatusDto statusDto)
        {
            try
            {
                var status = await _statusService.AddStatus(statusDto);

                return CreatedAtAction(nameof(Get), new {id=status.Id},status);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        // PUT api/<StatusesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, StatusDto statusDto)
        {
            try
            {
                await _statusService.UpdateStatus(id, statusDto);

                return NoContent();

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

        // DELETE api/<StatusesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _statusService.DeleteStatus(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    message = _errorMessage.UnexpectedErrorMessage(),
                    details = ex.Message
                });
            }
        }

       
    }
}
