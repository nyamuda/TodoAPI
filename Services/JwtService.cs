using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration) {
            _configuration = configuration;
        }

        
        public string GenerateJwtToken(string name, string email,string role="User")
        {
            var jwtSettings = _configuration.GetSection("Authentication:JwtSettings") ?? throw new Exception("Jwt settings not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, name), new Claim(ClaimTypes.Role,role), new Claim(ClaimTypes.Email,email) },
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }   

        
    }
}
