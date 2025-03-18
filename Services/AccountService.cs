using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Dtos.Account;

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
            bool userExists = await _context.Users.AnyAsync(u => u.Email.Equals(userRegisterDto.Email));
            if (userExists)
            {
                var message = "A user with this email is already registered.";
                throw new InvalidOperationException(message);
            }


            //hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            var user = new User
            {
                Name = userRegisterDto.Name,
                Email = userRegisterDto.Email,
                Phone=userRegisterDto.Phone,
                Password = hashedPassword,
                IsVerified = false,
                Role = "User"
            };

            //add the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

        }
        public async Task<(string accessToken, string refreshToken)> Login(UserLoginDto loginDto)
        {
            //access token lifespan is 72 hours  = 4320 minutes
            var accessTokenLifespan = 4320;

            //refresh token lifespan is 7 days  = 10080 minutes
            var refreshTokenLifespan = 10080;


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
            //token lifespan is 72 hours
            var accessToken = _jwtService.GenerateJwtToken(user:user,expiresIn: accessTokenLifespan);

            //create a refresh token
            //token lifespan is 7 days
            var refreshToken = _jwtService.GenerateJwtToken(user: user, expiresIn: refreshTokenLifespan);

            return (accessToken, refreshToken);
        }
   


        public async Task SendPasswordResetEmail(string email)
        {
            //check to see if user with the given email exists
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (userExists == null)
                throw new KeyNotFoundException("User with the provided email does not exist.");

            //generate reset token
            var token = _jwtService.GenerateJwtToken(user:userExists);

            string resetUrl = $"{_appService.AppDomainName}/account/password-reset?token={token}";


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
            var token = _jwtService.GenerateJwtToken(user: userExists);

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
