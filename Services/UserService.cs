using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Dtos.Account;
namespace TodoAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public UserService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        //get all users
        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        //get user information
        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            return user;
        }

        //update user
        public async Task UpdateUser(int id, UserUpdateDto userUpdateDto)
        {

            var user = await GetUser(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            //check if user's new email doesn't already exist
            bool emailExist = await _context.Users.AnyAsync(u => u.Email == userUpdateDto.Email && u.Id != id);

            if (emailExist)
                throw new InvalidOperationException("A user with this email already exists.");



            user.Name = userUpdateDto.Name;
            user.Email = userUpdateDto.Email;
            user.Phone = userUpdateDto.Phone;

            await _context.SaveChangesAsync();



        }



        public async Task DeleteUser(int id)
        {
            var user = await GetUser(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));

            if (user is null)
                throw new KeyNotFoundException("User with the given email does not exist.");

            return user;

        }

        
    }
}
