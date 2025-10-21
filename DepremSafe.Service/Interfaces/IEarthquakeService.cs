using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;

namespace DepremSafe.Service.Interfaces
{
    public interface IEarthquakeService
    {
        Task<EarthquakeDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<EarthquakeDTO>> GetAllAsync();
        Task AddAsync(EarthquakeDTO earthquakeDto);
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        public List<City> GetNearest10Cities(double depremLat, double depremLon, List<City> allCities);
    }
}
