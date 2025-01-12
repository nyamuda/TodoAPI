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


        public async Task<(bool isSuccess, string message)> RegisterUser(UserRegisterDto userRegisterDto)
        {
           try
            {
                //check if user with the provided email already exists
                bool userExists = _context.Users.Any(u => u.Email.Equals(userRegisterDto.Email));
                if (userExists) {
                    var message = "User with that email already exists.";
                    return (false, message);
                }

                //hash the password
                var hashedPassword= BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

                var user = new User
                {
                    Name = userRegisterDto.Name,
                    Email = userRegisterDto.Email,
                    Password = hashedPassword,
                    IsVerified=false,
                    Role="User"
                };

                //add the user
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, "User has been registered.");
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool isSuccess, string message)> LoginUser(UserLoginDto loginDto) { 

           try
            {
                //check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    var message = "User with the provided email does not exist.";
                    return (false, message);
                }

                // Compare the provided password with the stored hashed password
                string hashedPassword = user.Password;
                var isCorrectPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashedPassword);

                if (!isCorrectPassword) {
                    var message = "The provided password is incorrect.";
                    return (false, message);
                }

                //create token since the provided password is correct
                var token = _jwtService.GenerateJwtToken(user);

                return (true, token);

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

        }
    }
}
