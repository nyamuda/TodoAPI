using Microsoft.EntityFrameworkCore;
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


        //Get image by ID
        public async Task<Models.Image> GetImage(int id)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (image is null) throw new KeyNotFoundException($"Image with ID {id} does not exist.");

            return image;
        }

        //Get all images
        public async Task<List<Models.Image>> GetImages()
        {
            var images = await _context.Images.ToListAsync();
            return images;  
        }

        public async Task UpdateImage(int id)

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
