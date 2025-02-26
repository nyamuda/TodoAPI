using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Feature;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly FeatureService _featureService;
        private readonly ErrorMessageService _errorMessage;

        public FeaturesController(FeatureService featureService, ErrorMessageService errorMessage)
        {
            _featureService = featureService;
            _errorMessage = errorMessage;
        }

        // GET: api/<FeaturesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var features = await _featureService.GetFeatures();
                return Ok(features);
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

        // GET api/<FeaturesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var feature = await _featureService.GetFeature(id);
                return Ok(feature);
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

        // POST api/<FeaturesController>
        [HttpPost]
        public async Task<IActionResult> Post(FeatureDto featureDto)
        {
            try
            {
                var feature = await _featureService.AddFeature(featureDto);
                return CreatedAtAction(nameof(Get), new { id = feature.Id }, feature);
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

        // PUT api/<FeaturesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FeatureDto featureDto)
        {
            try
            {
                await _featureService.UpdateFeature(id, featureDto);
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

        // DELETE api/<FeaturesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _featureService.DeleteFeature(id);
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

        // GET api/<FeaturesController>/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var feature = await _featureService.GetFeatureByName(name);
                return Ok(feature);
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
