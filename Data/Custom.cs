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

        public double GetDistance(double latitude, double longitude, double otherLatitude, double otherLongitude)
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
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={startLatitude},{startLongitude}&destinations={endLatitude},{endLongitude}&key=your_google_map_key";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(content);

                    var distanceText = (string)jsonObject["rows"]?[0]?["elements"]?[0]?["distance"]?["text"]!;
                    if (distanceText != null && distanceText.EndsWith("km"))
                    {
                        // Extract distance in kilometers
                        var distanceInKilometers = double.Parse(distanceText.Replace(" km", ""));
                        return distanceInKilometers;
                    }
                    else
                    {
                        throw new FormatException("Distance not returned in expected format.");
                    }
                }
                else
                {
                    throw new HttpRequestException("Failed to get distance from the Google Maps API.");
                }
            }

        }
        public async Task<string> GetNearestLocationName(double latitude, double longitude)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&key=AIzaSyAK4M5dY-0P9pDc12nBdHnsmyCkFJMENSQ";

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    response.EnsureSuccessStatusCode(); // Throws exception for unsuccessful status codes

                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                    string nearestLocationName = data.results[0].name;

                    return nearestLocationName;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}