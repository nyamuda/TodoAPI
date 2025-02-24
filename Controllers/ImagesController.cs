using Firebase.Storage;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Images;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly FirebaseStorageService _firebaseStorageService;
        private readonly ImageService _imageService;
        private readonly string _bucketName = "drivingschool-7c02e.appspot.com";

        public ImagesController(FirebaseStorageService firebaseStorageService, IConfiguration config, ImageService imageService)
        {
            _firebaseStorageService = firebaseStorageService;
            _imageService = imageService;
        }
        // GET: api/<StatusesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var images = await _imageService.GetImages();
                return Ok(images);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET api/<StatusesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var image = await _imageService.GetImage(id);

                return Ok(image);
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
        // POST api/<ImagesController>
        [HttpPost]
        // [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Post(UploadImageDto uploadDto)
        {
            try
            {
                IFormFile file = uploadDto.File;
                var fileCategory = uploadDto.Category?.ToLower();

                if (file is null || file.Length == 0) throw new InvalidOperationException("Invalid file.");

                //max length is 5MB
                if (file.Length > 5 * 1024 * 1024) throw new InvalidOperationException("File size cannot exceed 5MB.");

                //check if the image is a valid image or not
                if (_imageService.IsImageValid(file) is false)
                    throw new InvalidOperationException("Unsupported file. Please choose a valid image to upload.");


                //upload the image to Firebase and get the url
                var fileUrl = await _firebaseStorageService.UploadFileAsync(file: file, category: fileCategory);

                //save the image information to the database
                var addImageDto = new AddImageDto()
                {
                    Url = fileUrl,
                    FileName = file.FileName,
                    Category = fileCategory,
                    Description = uploadDto.Description
                };

                var image = await _imageService.AddImage(addImageDto);

                return CreatedAtAction(nameof(Get), new { id = image.Id }, image);

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
                return StatusCode(500, new { Message = ex.ToString() });
            }
        }

        // PUT api/<ImagesController>/5
        //[HttpPut("{id}")]
        //public void Put()
        //{
        //}

       // DELETE api/<ImagesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //first, get the image details
                var image = await _imageService.GetImage(id);

                //second, delete the image on Firebase
                _firebaseStorageService.DeleteFileysnc(image);

                return NoContent();

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
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
