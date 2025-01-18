using Microsoft.EntityFrameworkCore;
using RestSharp;
using static Org.BouncyCastle.Math.EC.ECCurve;
using TodoAPI.Dtos.Account;
using TodoAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using TodoAPI.Data;

namespace TodoAPI.Services
{
    public class FacebookService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public FacebookService(IConfiguration config, ApplicationDbContext context, JwtService jwtService)
        {

            _config = config;
            _context = context;
            _jwtService = jwtService;
        }
        //Exchange code for access token
        public async Task<string> GetFacebookToken(string code)
        {
            var facebookSettings = _config.GetSection("Authentication:Facebook");

            var clientId = facebookSettings["ClientId"];
            var secret = facebookSettings["ClientSecret"];
            var redirectUri = facebookSettings["RedirectUrl"];



            var restClient = new RestClient("https://graph.facebook.com/v21.0/oauth/access_token?");

            // Create a new request
            var request = new RestRequest()
                .AddParameter("client_id", clientId)
                .AddParameter("client_secret", secret)
                .AddParameter("code", code)
                .AddParameter("redirect_uri", redirectUri);
                

            // Set the method as GET
            request.Method = Method.Get;

            //execute the request
            var response = await restClient.ExecuteAsync<FacebookTokenResponseDto>(request);



            if (!response.IsSuccessful)
                throw new Exception($"Error getting Facebook token: {response.ErrorMessage}");


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

        public async Task<FacebookUser> GetFacebookUserInfo(string token)
        {
            var client = new RestClient("https://graph.facebook.com/v21.0/me");

            var request = new RestRequest()

            .AddParameter("fields", "name,email")
                .AddParameter("access_token", token);


            var response = await client.ExecuteAsync<FacebookUser>(request);


            if (!response.IsSuccessful)
            {
                throw new Exception($"Error getting Facebook user details: {response.ErrorMessage}");
            }

            if (response.Data == null)
                throw new InvalidOperationException("The response data was null");

            return response.Data;
        }

        public async Task<string> FacebookLogin(FacebookUser facebookUser)
        {
            string token = "";
            //check if the user with that email exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(facebookUser.Email));


            //if user with the provided email does not exits
            //register the user instead
            if (user == null)
            {
                token = await FacebookRegister(facebookUser);
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
        public async Task<string> FacebookRegister(FacebookUser facebookUser)
        {
            //check if the user with that email already exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(facebookUser.Email));

            //if they exist, then you can't register
            if (userExists != null)
            {
                var message = "An account with the provided email already exists. Please log in using your existing credentials.";
                throw new InvalidOperationException(message);
            }


            var user = new User
            {
                Name = facebookUser.Name,
                Email = facebookUser.Email,
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
