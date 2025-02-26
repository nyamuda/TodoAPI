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
        public async Task<IActionResult> Get()
        {
            try
            {
                var statuses = await _statusService.GetStatuses();
                return Ok(statuses);
            }
            catch (Exception ex) {

                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET api/<StatusesController>/5
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var status = await _statusService.GetStatusByName(name);

                return Ok(status);
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
