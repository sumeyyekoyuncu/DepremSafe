using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;

namespace DepremSafe.Service.Services
{
    public class UserLocationService
    {
        private readonly IUserLocationRepository _locationRepository;
        public UserLocationService(IUserLocationRepository locationRepository) => _locationRepository = locationRepository;

        public Task<UserLocation> GetByIdAsync(Guid id) => _locationRepository.GetByIdAsync(id);
        public Task<IEnumerable<UserLocation>> GetByUserIdAsync(Guid userId) => _locationRepository.GetByUserIdAsync(userId);
        public Task AddAsync(UserLocation location) => _locationRepository.AddAsync(location);
    
}
}
