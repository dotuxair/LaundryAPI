using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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

        public double GetDistance( double latitude, double longitude, double otherLatitude , double otherLongitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            // Earth's radius in kilometers
            const double earthRadiusKm = 6371.0;

            // Distance in kilometers
            return earthRadiusKm * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        public async Task<double> GetDrivingDistanceAsync(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
        {
            var accessToken = "pk.eyJ1IjoidXphaXJpamF6IiwiYSI6ImNrdGxmeHlvNDBnN3kybm1qZWV2OGtqd28ifQ.AvsQhG01rpc8kOr3qpCX9A";
            var url = $"https://api.mapbox.com/directions/v5/mapbox/driving/{startLongitude},{startLatitude};{endLongitude},{endLatitude}?access_token={accessToken}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(content);

                    var distanceInMeters = (double)jsonObject["routes"]![0]!["distance"]!;
                    var distanceInKilometers = distanceInMeters / 1000;

                    return distanceInKilometers;
                }
                else
                {
                    throw new HttpRequestException("Failed to get distance from the Mapbox API.");
                }
            }
        }

    }
}
