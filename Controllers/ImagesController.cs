using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly FirebaseStorage _firebaseStorage;
        private readonly ImageService _imageService;

        public ImagesController(FirebaseStorage firebaseStorage, IConfiguration config, ImageService imageService)
        {
            var bucket = config.GetSection("Authentication:Firebase:Bucket").Value;
            _firebaseStorage = new FirebaseStorage(bucket);
            _imageService = imageService;
        }
        // GET: api/<ImagesController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<ImagesController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ImagesController>
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Post(IFormFile file, IConfiguration config)
        {
            try
            {
                if (file is null || file.Length == 0) throw new InvalidOperationException("Invalid file.");

                //max length is 5MB
                if (file.Length > 5 * 1024 * 1024) throw new InvalidOperationException("File size cannot exceed 5MB.");

                //check if the image is a valid image or not
                if (_imageService.IsImageValid(file) is false) 
                    throw new InvalidOperationException("Invalid file. Only image files are allowed.");


                //generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name);

                //save image to Firebase storage
                //and get the URL
                using var stream = file.OpenReadStream();
                var downloadUrl = await _firebaseStorage.Child("carwash-images").Child(fileName).PutAsync(stream);

                return StatusCode(201, new {Message="Image has been successfully uploaded.",ImageUrl=downloadUrl});

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
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

        // PUT api/<ImagesController>/5
        //[HttpPut("{id}")]
        //public void Put()
        //{
        //}

        // DELETE api/<ImagesController>/5
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            try
            {
                await _firebaseStorage.Child("carwash-images").Child(fileName).DeleteAsync();

                return NoContent();

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
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
    }
}
