using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using TodoAPI.Data;

namespace TodoAPI.Services
{
    public class ImageService
    {
        private ApplicationDbContext _context;
        public ImageService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Image> GetImage(int id)
        {
            var image=await _context.
        }

        //check whether an image is a real and valid image or not
        public bool IsImageValid(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                //if the file is not an a valid image
                //then the following line will throw an exception
                //e.g InvalidImageContentException
                using var image = Image.Load(stream);

                // If no exception is thrown, it's a valid image
                return true;
            }
            catch
            {

                return false;
            }
        }
    }
}
