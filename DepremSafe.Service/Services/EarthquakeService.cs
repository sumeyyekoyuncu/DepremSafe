using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;
using DepremSafe.Service.Interfaces;

namespace DepremSafe.Service.Services
{
    public class EarthquakeService:IEarthquakeService
    {
        private readonly IEarthquakeRepository _earthquakeRepository;
        private readonly IMapper _mapper;

        public EarthquakeService(IEarthquakeRepository earthquakeRepository, IMapper mapper)
        {
            _earthquakeRepository = earthquakeRepository;
            _mapper = mapper;
        }

        public async Task<EarthquakeDTO> GetByIdAsync(Guid id)
        {
            var earthquake = await _earthquakeRepository.GetByIdAsync(id);
            return _mapper.Map<EarthquakeDTO>(earthquake);
        }

        public async Task<IEnumerable<EarthquakeDTO>> GetAllAsync()
        {
            var earthquakes = await _earthquakeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EarthquakeDTO>>(earthquakes);
        }

        public async Task AddAsync(EarthquakeDTO earthquakeDto)
        {
            var earthquake = _mapper.Map<Earthquake>(earthquakeDto);
            await _earthquakeRepository.AddAsync(earthquake);
        }
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Dünya yarıçapı km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        public List<City> GetNearest10Cities(double depremLat, double depremLon, List<City> allCities)
        {
            return allCities
                .OrderBy(c => CalculateDistance(depremLat, depremLon, c.Latitude, c.Longitude))
                .Take(10)
                .ToList();
        }

    }
}
