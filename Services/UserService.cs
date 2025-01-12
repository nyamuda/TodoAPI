using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
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


        public async Task RegisterUser(UserRegisterDto userRegisterDto)
        {
            //check if user with the provided email already exists
            bool userExists = _context.Users.Any(u => u.Email.Equals(userRegisterDto.Email));
            if (userExists)
            {
                var message = "User with that email already exists.";
                throw new KeyNotFoundException(message);
            }

            //hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            var user = new User
            {
                Name = userRegisterDto.Name,
                Email = userRegisterDto.Email,
                Password = hashedPassword,
                IsVerified = false,
                Role = "User"
            };

            //add the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();



        }

        public async Task<string> LoginUser(UserLoginDto loginDto)
        {


            //check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                var message = "User with the provided email does not exist.";
                throw new KeyNotFoundException(message);
            }

            // Compare the provided password with the stored hashed password
            string hashedPassword = user.Password;
            var isCorrectPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashedPassword);

            if (!isCorrectPassword)
            {
                var message = "The provided password is incorrect.";
                throw new UnauthorizedAccessException(message);
            }

            //create token since the provided password is correct
            var token = _jwtService.GenerateJwtToken(user);

            return token;


        }

        //get user information
        public async Task<User?> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

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
            var emailExist = await _context.Users.AnyAsync(u => u.Email == user.Email && user.Id != id);

            if (emailExist)
                throw new InvalidOperationException("A user with this email already exists.");


            user.Name = userUpdateDto.Name;
            user.Email = userUpdateDto.Email;

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
    }
}
