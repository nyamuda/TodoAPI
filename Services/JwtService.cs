namespace TodoAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration) {
            _configuration = configuration;
        }

        /*
        public string GenerateJwtToken(string userId)
        {
            var jwtSettings = _configuration.GetSection("Authentication:JwtSettings") ?? throw new Exception("Jwt settings not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }   

        */
    }
}
