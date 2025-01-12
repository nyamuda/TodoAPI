using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using BCrypt.Net;
namespace TodoAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
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
                    Password = hashedPassword
                };

                //add the user
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, "User has been registered");
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
