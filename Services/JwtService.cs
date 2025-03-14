﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration) {
            _configuration = configuration;
        }

        
        public string GenerateJwtToken(User user, double expiresIn =15)
        {
           
            var jwtSettings = _configuration.GetSection("Authentication:JwtSettings") ?? throw new Exception("Jwt settings not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
          
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, user.Name), new Claim(ClaimTypes.Role,user.Role), new Claim(ClaimTypes.Email,user.Email), new Claim("isVerified",user.IsVerified.ToString()) },
                //default token lifespan is  = 15 minutes
                expires: DateTime.Now.AddMinutes(expiresIn),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public ClaimsPrincipal ValidateToken(string token)
        {
            var jwtSettings = _configuration.GetSection("Authentication:JwtSettings") ?? throw new Exception("Jwt settings not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var tokenHandler = new JwtSecurityTokenHandler();



            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey =key
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (principal == null)
                throw new InvalidOperationException("The provided token is invalid or has expired.");

            return principal;
        }




    }
}
