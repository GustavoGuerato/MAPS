using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeoLocationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoLocationController : ControllerBase
    {
        private readonly string apiUrl = "https://googlelatlog.azurewebsites.net/api/http_trigger?code=_Ka380mEoqT2bUQWBiG8olL2phzfPJPpJLZGp375kLCiAzFuouiTNw%3D%3D";

        [HttpPost]
        [Route("GetCoordinates")]
        public async Task<IActionResult> GetCoordinates([FromBody] AddressRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Endereco))
            {
                return BadRequest("O endereço não pode ser vazio.");
            }

            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { endereco = request.Endereco }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var coordinates = JsonConvert.DeserializeObject<CoordinatesResponse>(result);
                    return Ok(coordinates);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Erro ao chamar a API de geocodificação.");
                }
            }
        }
    }

    public class AddressRequest
    {
        public required string Endereco { get; set; }
    }

    public class CoordinatesResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
