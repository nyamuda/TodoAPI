using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly FirebaseStorage _firebaseStorage;

        public ImagesController(FirebaseStorage firebaseStorage, IConfiguration config)
        {
            var bucket = config.GetSection("Authentication:Firebase:Bucket").Value;
            _firebaseStorage = new FirebaseStorage(bucket);
        }
        // GET: api/<ImagesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ImagesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ImagesController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(IFormFile file, IConfiguration config)
        {
            try
            {
                if (file is null || file.Length == 0) throw new InvalidOperationException("Invalid file.");

                //max length is 5MB
                if (file.Length > 5 * 1024 * 1024) throw new InvalidOperationException("File size cannot exceed 5MB.");

                //generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name);

                //save image to Firebase storage

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PUT api/<ImagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ImagesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
