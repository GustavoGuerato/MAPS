using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoLocationAPI.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly string apiUrl = "https://googlelatlog.azurewebsites.net/api/http_trigger?code=_Ka380mEoqT2bUQWBiG8olL2phzfPJPpJLZGp375kLCiAzFuouiTNw%3D%3D";

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371000; 
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string endereco)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { endereco = endereco }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var geoData = JsonConvert.DeserializeObject<GeoResponse>(result);
                    return (geoData?.Latitude ?? 0, geoData?.Longitude ?? 0);
                }
                else
                {
                    return null; 
                }
            }
        }
    }
}
