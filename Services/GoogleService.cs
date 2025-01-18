using Microsoft.EntityFrameworkCore;
using RestSharp;
using TodoAPI.Data;
using TodoAPI.Dtos.Account;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class GoogleService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public GoogleService(IConfiguration config, ApplicationDbContext context, JwtService jwtService)
        {

            _config = config;
            _context = context;
            _jwtService = jwtService;
        }

        //Exchange code for access token
        public async Task<string> GetGoogleToken(string code)
        {
            var googleSettings = _config.GetSection("Authentication:Google");

            var clientId = googleSettings["ClientId"];
            var secret = googleSettings["ClientSecret"];
            var redirectUri = googleSettings["RedirectUrl"];



            var restClient = new RestClient("https://oauth2.googleapis.com/token");

            // Create a new request and set the method to POST as a string
            var request = new RestRequest()
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("client_id", clientId)
                .AddParameter("client_secret", secret)
                .AddParameter("code", code)
                .AddParameter("redirect_uri", redirectUri)
                .AddParameter("grant_type", "authorization_code");

            // Set the method as POST
            request.Method = Method.Post;

            //execute the request
            var response = await restClient.ExecuteAsync<GoogleTokenResponseDto>(request);



            if (!response.IsSuccessful)
                throw new Exception($"Error getting Google token: {response.ErrorMessage}");


            if (response.Content == null)
                throw new InvalidOperationException("The response content was null.");


            if (response.Data != null)
            {
                var accessToken = response.Data.Access_Token;

                return accessToken;
            }
            else
                throw new InvalidOperationException("The response content was null.");




        }

        public async Task<GoogleUser> GetGoogleUserInfo(string token)
        {
            var client = new RestClient("https://www.googleapis.com/oauth2/v3/userinfo");

            var request = new RestRequest()
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"Bearer {token}");


            var response = await client.ExecuteAsync<GoogleUser>(request);


            if (!response.IsSuccessful)
            {
                throw new Exception($"Error getting Google user details: {response.ErrorMessage}");
            }

            if (response.Data == null)
                throw new InvalidOperationException("The response data was null");

            return response.Data;
        }

        public async Task<string> GoogleLogin(GoogleUser googleUser)
        {
            string token = "";
            //check if the user with that email exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(googleUser.Email));


            //if user with the provided email does not exits
            //register the user instead
            if (user == null)
            {
                token = await GoogleRegister(googleUser);
                return token;
            }

            else
            {
                //create JWT token

                token = _jwtService.GenerateJwtToken(user);

                return token;
            }

        }

        //Register user via Google
        public async Task<string> GoogleRegister(GoogleUser googleUser)
        {
            //check if the user with that email already exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(googleUser.Email));

            //if they exist, then you can't register
            if (userExists != null)
            {
                var message = "An account with the provided email already exists. Please log in using your existing credentials.";
                throw new InvalidOperationException(message);
            }


            var user = new User
            {
                Name = googleUser.Name,
                Email = googleUser.Email,
                IsVerified = true,
                Role = "User"
            };


            _context.Add(user);
            await _context.SaveChangesAsync();

            //create JWT token
            var token = _jwtService.GenerateJwtToken(user);

            return token;

        }
    }
}
