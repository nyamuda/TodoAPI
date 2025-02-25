using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Booking;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    //Car Wash Service Types
    public class ServiceTypesService
    {

        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;

        public ServiceTypesService(ApplicationDbContext context,ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }


        // Add a new booking service type
        public async Task<ServiceType> AddServiceType(ServiceTypeDto serviceTypeDto)
        {
            //check if the image with the given ID really exists
            var image = _imageService.GetImage(serviceTypeDto.ImageId);

            var serviceType = new ServiceType()
            {
                Name = serviceTypeDto.Name,
                Price = serviceTypeDto.Price,
                Overview=serviceTypeDto.Overview,
                Description=serviceTypeDto.Description,
                ImageId=image.Id
            };

            _context.Add(serviceType);
            await _context.SaveChangesAsync();

            return serviceType;

        }
        //Update service type
        public async Task UpdateServiceType(int id, ServiceTypeDto serviceTypeDto)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            serviceType.Name = serviceTypeDto.Name;
            serviceType.Price = serviceTypeDto.Price;

            await _context.SaveChangesAsync();
        }

        //Delete service type
        public async Task DeleteServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            _context.Remove(serviceType);
            await _context.SaveChangesAsync();

        }

        //Get all service types
        public async Task<List<ServiceType>> GetServiceTypes()
        {
            var serviceTypes = await _context.ServiceTypes.Include(x=>x.Image).ToListAsync();
            return serviceTypes;
        }

        //Get service type by ID
        public async Task<ServiceType> GetServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (serviceType is null)
                throw new KeyNotFoundException("Service type with the given ID does not exist.");

            return serviceType;

        }

    }
}
