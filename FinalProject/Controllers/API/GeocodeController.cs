using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinalProject.Controllers.API
{
    [ApiController]
    [Route("api/geocode")]
    public class GeocodeController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleMaps:ApiKey"];
        }

        [HttpGet("getCoordinates")]
        public async Task<ActionResult<CoordinatesResponseVM>> GetCoordinates(string address)
        {
            var (latitude, longitude, formattedAddress) = await GeocodeAddressAsync(address);

            if (latitude == 0 && longitude == 0)
            {
                return NotFound("Coordinates not found for the given address.");
            }

            return Ok(new CoordinatesResponseVM
            {
                Latitude = latitude,
                Longitude = longitude,
                FormattedAddress = formattedAddress 
            });
        }


        private async Task<(double latitude, double longitude, string formattedAddress)> GeocodeAddressAsync(string address)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(jsonResponse);

                if (result["results"].HasValues)
                {
                    var location = result["results"][0]["geometry"]["location"];
                    double latitude = location["lat"].Value<double>();
                    double longitude = location["lng"].Value<double>();
                    string formattedAddress = result["results"][0]["formatted_address"].Value<string>(); 
                    return (latitude, longitude, formattedAddress);
                }
            }
            return (0, 0, null); 
        }

    }
}
