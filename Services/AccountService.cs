using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using RestSharp;

namespace TodoAPI.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;

        public AccountService(ApplicationDbContext context, JwtService jwtService, IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;

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

        //Exchange code for access token
       public async Task<string?> GetGoogleToken(string code)
        {
            var googleSettings = _config.GetSection("Authentication:Google");

            var cliendId = googleSettings["ClientId"];
            var secret = googleSettings["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"];
            var redirectUrl = googleSettings["RedirectUrl"];

            var restClient = new RestClient("https://oauth2.googleapis.com/token");

            var request = new RestRequest().AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add URL-encoded parameters
            request.AddParameter("client_id", cliendId);
            request.AddParameter("client_secret", secret);
            request.AddParameter("code", code);
            request.AddParameter("redirect_url", redirectUrl);
            request.AddParameter("grant_type", "authorization_code");

            //execute the request
            var response = await restClient.ExecuteAsync(request);

            if(!response.IsSuccessful)
                throw new Exception($"Error getting Google token: {response.ErrorMessage}");

            return response.Content;
        }


    }
}
