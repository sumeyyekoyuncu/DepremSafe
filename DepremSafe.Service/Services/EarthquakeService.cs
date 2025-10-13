using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;

namespace DepremSafe.Service.Services
{
    public class EarthquakeService
    {
        private readonly IEarthquakeRepository _earthquakeRepository;
        public EarthquakeService(IEarthquakeRepository earthquakeRepository) => _earthquakeRepository = earthquakeRepository;

        public Task<Earthquake> GetByIdAsync(Guid id) => _earthquakeRepository.GetByIdAsync(id);
        public Task<IEnumerable<Earthquake>> GetAllAsync() => _earthquakeRepository.GetAllAsync();
        public Task AddAsync(Earthquake earthquake) => _earthquakeRepository.AddAsync(earthquake);
    }
}
