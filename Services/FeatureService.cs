using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Booking;
using TodoAPI.Dtos.Feature;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class FeatureService
    {
        private readonly ApplicationDbContext _context;

        public FeatureService(ApplicationDbContext context) { _context = context; }



        //Get feature by ID
        public async Task<Feature> GetFeature(int id)
        {
            var feature = await _context.Features.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (feature is null)
                throw new KeyNotFoundException($"Feature with ID {id} does not exist.");

            return feature;
        }

        //Get all features
        public async Task<(List<Feature> features, PageInfo pageInfo)> GetFeatures(int page, int pageSize)
        {
            List<Feature> features = await _context.Features
                .Skip((page-1)*pageSize)
                .OrderByDescending(x=>x.CreatedAt)
                .Take(pageSize)
                .ToListAsync();

            //total features
            var totalFeatures = await _context.Features.CountAsync();
            //is there still more features
            bool hasMoreFeatures = totalFeatures > pageSize * page;

            var pageInfo = new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                HasMore = hasMoreFeatures
            };

            return (features, pageInfo);    
        }

        //Add a feature

        public async Task<Feature> AddFeature(FeatureDto featureDto)
        {
            //check to see if a feature with a given name does not already exist
            var featureWithNameExist = await _context.Features.FirstOrDefaultAsync(x => x.Name.Equals(featureDto.Name.ToLower()));

            //if feature with the given name already exists, throw an exception
            if (featureWithNameExist is not null)
                throw new InvalidOperationException($"Feature with name {featureDto.Name} already exists.");

            Feature feature = new Feature
            {
                Name = featureDto.Name.ToLower()
            };

            _context.Features.Add(feature);

            await _context.SaveChangesAsync();

            return feature;

        }
        //Update a feature with a given ID
        public async Task UpdateFeature(int id, FeatureDto featureDto)
        {
            //get the feature
            Feature feature = await GetFeature(id);

            //update it
            feature.Name = featureDto.Name;
            feature.Description = featureDto.Description;

            await _context.SaveChangesAsync();

        }
        //Delete a feature with a given ID
        public async Task DeleteFeature(int id)
        {
            Feature feature = await GetFeature(id);

            _context.Remove(feature);
            await _context.SaveChangesAsync();
        }


        //get feature by name
        public async Task<Feature> GetFeatureByName(string name)
        {
            var feature = await _context.Features.FirstOrDefaultAsync(x => x.Name.Equals(name.ToLower()));
            if (feature is null)
                throw new KeyNotFoundException($@"Feature with name ""{name}"" does not exist.");
            return feature;

        }

        
    }
}