using System.Threading.Tasks;

namespace GeoLocationAPI.Services
{
    public interface IGeoLocationService
    {
        Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string endereco);

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
