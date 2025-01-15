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
        private readonly AppService _appService;
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;

        public AccountService(ApplicationDbContext context, JwtService jwtService, 
            IConfiguration config, AppService appService, TemplateService templateService, EmailSender emailSender)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
            _appService = appService;
            _templateService = templateService;
            _emailSender = emailSender;
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
       public async Task<string> GetGoogleToken(string code)
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


            if (response.Content == null)
                throw new InvalidOperationException("The response content was null.");

            return response.Content;
        }

        public async Task<GoogleUser> GetGoogleUserInfo(string token)
        {
            var client = new RestClient("https://www.googleapis.com/oauth2/v3/userinfo");

            var request = new RestRequest()
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"Bearer {token}");


            var response = await client.ExecuteAsync<GoogleUser>(request);

            if(!response.IsSuccessful)
            {
                throw new Exception($"Error getting Google user details: {response.ErrorMessage}");
            }

            if (response.Data == null)
                throw new InvalidOperationException("The response data was null");

            return response.Data;
        }

        public async Task<string> GoogleLogin(GoogleUser googleUser)
        {
            //check if the user with that email exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(googleUser.Email));

            if (user == null)
            {
                var message = "User with the provided email does not exist.";
                throw new KeyNotFoundException(message);
            }

            //create JWT token

            var token = _jwtService.GenerateJwtToken(user);

            return token;
                
        }

        //Register user via Google
        public async Task<string> GoogleRegister(GoogleUser googleUser)
        {
            //check if the user with that email already exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(googleUser.Email));

            //if they exist, then you can't register
            if (userExists != null)
            {
                var message = "User with the provided email already exists.";
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


        public async Task SendPasswordResetEmail(string email)
        {
            //check to see if user with the given email exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (userExists == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //generate reset token
            var token = _jwtService.GenerateJwtToken(user:userExists,duration:"short");

            string resetUrl = $"{_appService.AppDomainName}/account/password/reset?token={token}";


            string htmlTemplate = _templateService.ResetPassword(resetUrl,userExists.Name);


            //send email to reset password
            var emailSubject = "Password Reset";
            await _emailSender.SendEmail(userExists.Name, email, emailSubject, htmlTemplate);

        }

        //Reset password by validating token
        public async Task ResetPassword(string email, string newPassword)
        {
            //check if user with the provided email already exists
           var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (userExists==null)
            {
                var message = "User with the provided email does not exists.";
                throw new InvalidOperationException(message);
            }

            //hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            //update the password
            userExists.Password = hashedPassword;
            await _context.SaveChangesAsync();

        }

        public async Task SendEmailVerification(string email)
        {
            //check to see if user with the given email exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (userExists == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //generate reset token
            var token = _jwtService.GenerateJwtToken(user: userExists, duration: "short");

            string confirmUrl = $"{_appService.AppDomainName}/account/verify?token={token}";


            string htmlTemplate = _templateService.ConfirmEmail(confirmUrl, userExists.Name);


            //send email to reset password
            var emailSubject = "Email Confirmation";
            await _emailSender.SendEmail(userExists.Name, email, emailSubject, htmlTemplate);

        }


        public async Task VerifyAccount(string email)
        {
            //check to see if user with the given email exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (userExists == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            userExists.IsVerified = true;
            await _context.SaveChangesAsync();
        }
    }
}
