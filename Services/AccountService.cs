using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AccountService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task Register(UserRegisterDto userRegisterDto)
        {
            //check if user with the provided email already exists
            bool userExists = _context.Users.Any(u => u.Email.Equals(userRegisterDto.Email));
            if (userExists)
            {
                var message = "User with that email already exists.";
                throw new InvalidOperationException(message);
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
        public async Task<string> Login(UserLoginDto loginDto)
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
    }
}
