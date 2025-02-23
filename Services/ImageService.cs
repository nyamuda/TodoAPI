using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using TodoAPI.Data;
using TodoAPI.Dtos.Images;

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


        //Add a new image
        public async Task<Models.Image> AddImage(AddImageDto imageDto)
        {
            var image = new Models.Image()
            {
                Url = imageDto.Url,
                FileName = imageDto.FileName,
                Category = imageDto.Category,
                Description = imageDto.Description
            };
            _context.Add(image);

            await _context.SaveChangesAsync();

            return image;

        }

        //Update an image with a given ID
        public async Task UpdateImage(int id, UpdateImageDto imageDto)
        {
            //get the image with the given ID
            var image=await GetImage(id);

            //Update the image
            image.Url=imageDto.Url;
            image.FileName = imageDto.FileName;
            image.Category = imageDto.Category;
            image.Description=imageDto.Description;

            await _context.SaveChangesAsync();
        }

        //Delete and image with a given ID
        public async Task DeleteImage(int id)
        {
            //first, get the image
            var image = await GetImage(id);

            //if the image exists, delete it
            _context.Remove(image);

            await _context.SaveChangesAsync();
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
