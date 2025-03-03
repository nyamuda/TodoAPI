using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
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

        public ServiceTypesService(ApplicationDbContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }


        // Add a new booking service type
        public async Task<ServiceType> AddServiceType(ServiceTypeDto serviceTypeDto)
        {
            //check if the image with the given ID really exists
            var image = await _imageService.GetImage(serviceTypeDto.ImageId);

            //get all features that have given feature IDs
            //these IDs are IDs of the features that were selected
            //by the client for the service
            var selectedServiceFeatures = await _context.Features
                .Where(x => serviceTypeDto.FeatureIds.Contains(x.Id)).ToListAsync();

            //make sure the number of given feature IDs match 
            //the number of returned features with those IDs
            if (serviceTypeDto.FeatureIds.Count != selectedServiceFeatures.Count)
                throw new InvalidOperationException("One or more features are invalid.");

            var serviceType = new ServiceType()
            {
                Name = serviceTypeDto.Name,
                Price = serviceTypeDto.Price,
                Duration = serviceTypeDto.Duration,
                Overview = serviceTypeDto.Overview,
                Description = serviceTypeDto.Description,
                ImageId = image.Id
            };

            serviceType.Features.AddRange(selectedServiceFeatures);

            _context.ServiceTypes.Add(serviceType);
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
            var serviceTypes = await _context.ServiceTypes
                .OrderByDescending(x => x.Bookings.Count)
                .Include(x => x.Feedback)
                .Include(x => x.Image)
                .Include(x => x.Features)
                .ToListAsync();
            return serviceTypes;
        }

        //Get service type by ID
        public async Task<ServiceType> GetServiceType(int id)
        {
            var serviceType = await _context.ServiceTypes.Include(x => x.Image).Include(x => x.Feedback).Include(x => x.Features).FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (serviceType is null)
                throw new KeyNotFoundException($"Service type with the ID {id} does not exist.");

            return serviceType;

        }

        //Get the most popular service
        public async Task<ServiceType> GetPopularServiceType()
        {
            //the query
            var query = _context.ServiceTypes.AsQueryable();

            //Now, the most popular service type is the one with the most bookings
            //So, let's find the service with the most bookings by:
            //First, ordering (in descending order) the service types by number of bookings they have &
            //Second, taking the first service type from that list
            //In other words, the most popular service type will be the one at the top of that list
            var popularService = await query
                 .OrderByDescending(st => st.Bookings.Count)
                 .FirstOrDefaultAsync();

            //if there no service type found,
            //then it means no service types exist at all in the database
            if (popularService is null)
                throw new KeyNotFoundException("No car wash service types found in the database.");

            return popularService;

        }

        //Get all the feedback for a service type
        public async Task<(List<Feedback> feedback, PageInfo pageInfo, double averageRating)> GetServiceTypeFeedback(int page, int pageSize, int serviceTypeId)
        {
            //check if a service with the given serviceTypeId exists
            ServiceType serviceType = await GetServiceType(serviceTypeId);


            //get the feedback for a particular service type  
            var feedback = await _context.Feedback
                .Where(x => x.ServiceTypeId.Equals(serviceTypeId))
                .Include(x => x.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //total feedback
            int totalFeedback = await _context.Feedback
                .Where(x => x.ServiceTypeId.Equals(serviceTypeId))
                .CountAsync();
            //has more feedback 
            bool hasMore = totalFeedback > page * pageSize;


            //average rating rounded to 2 decimals
            double averageRating = await GetFeedbackAverageRating(serviceTypeId);

            //page info for pagination
            var pageInfo = new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMore
            };

            return (feedback, pageInfo, averageRating);
        }

        //Get the average rating of all the feedback for a particular service type
        public async Task<double> GetFeedbackAverageRating(int serviceTypeId)
        {
            //total number of all feedback
            var totalFeedback = await _context.Feedback
                .Where(x => serviceTypeId.Equals(serviceTypeId))
                .CountAsync();

            //total sum of the ratings
            double totalSumOfRatings = await _context.Feedback
                .Where(x => x.ServiceTypeId.Equals(serviceTypeId))
                .SumAsync(x => x.Rating);


            //calculate average rating
            double averageRating = Math.Round(totalSumOfRatings / totalFeedback, 2, MidpointRounding.AwayFromZero);

            return averageRating;
        }

    }
}
