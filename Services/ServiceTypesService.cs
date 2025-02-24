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

        public ServiceTypesService(ApplicationDbContext context)
        {
            _context = context;
        }


        // Add a new booking service type
        public async Task<ServiceType> AddServiceType(ServiceTypeDto serviceTypeDto)
        {

            var serviceType = new ServiceType()
            {
                Name = serviceTypeDto.Name,
                Price = serviceTypeDto.Price,
                Image=serviceTypeDto.Image
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
            var serviceTypes = await _context.ServiceTypes.ToListAsync();
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
