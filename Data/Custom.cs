using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FYP.API.Data
{
    public class Custom
    {
        private readonly IConfiguration _configuration;
        public Custom(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(TokenDto claim)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email , claim.Email),
                new Claim(ClaimTypes.Role,claim.Role),
            };

            string key = _configuration["Jwt:Key"]!.ToString();

            var credentials = new SigningCredentials(
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    SecurityAlgorithms.HmacSha256Signature);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Issuer"]
            };



            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            // Earth's radius in kilometers
            const double earthRadiusKm = 6371.0;

            return earthRadiusKm * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

    }
}
