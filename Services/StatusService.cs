using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Booking;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class StatusService
    {
        private readonly ApplicationDbContext _context;

        public StatusService(ApplicationDbContext context) { _context = context; }



        //Get status by ID
        public async Task<Status> GetStatus(int id)
        {
            var status = await _context.Statuses.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (status is null)
                throw new KeyNotFoundException($"Status with ID {id} does not exist.");

            return status;  
        }

        //Get all statuses
        public async Task<List<Status>> GetStatuses()
        {
            List<Status> statuses = await _context.Statuses.ToListAsync();
            return statuses;    
        }

        //Add a status

        public async Task<Status> AddStatus(StatusDto statusDto)
        {
            //check to see if a status with a given name does not already exist
            var statusWithNameExist=await _context.Statuses.FirstOrDefaultAsync(x => x.Name.Equals(statusDto.Name.ToLower()));

            //if status with the given name already exists, throw an exception
            if (statusWithNameExist is not null)
                throw new InvalidOperationException($"Status with name {statusDto.Name} already exists.");

            Status status = new Status
            {
                Name = statusDto.Name.ToLower()
            };

            _context.Statuses.Add(status);

            await _context.SaveChangesAsync();

            return status;

        }
        //Update a status with a given ID
        public async Task UpdateStatus(int id, StatusDto statusDto)
        {
            //get the status
            Status status = await GetStatus(id);

            //update it
            status.Name = statusDto.Name;

            await _context.SaveChangesAsync();

        }
        //Delete a status with a given ID
        public async Task DeleteStatus(int id)
        {
            Status status = await GetStatus(id);

            _context.Remove(status);
           await _context.SaveChangesAsync();
        }
    }
}
