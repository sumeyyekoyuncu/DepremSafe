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
    public class UserLocationService: IUserLocationService
    {
        private readonly IUserLocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public UserLocationService(IUserLocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }
      
        public async Task<UserLocationDTO> GetByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            return _mapper.Map<UserLocationDTO>(location);
        }

        public async Task<IEnumerable<UserLocationDTO>> GetByUserIdAsync(Guid userId)
        {
            var locations = await _locationRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<UserLocationDTO>>(locations);
        }

        public async Task AddAsync(UserLocationDTO locationDto)
        {
            var location = _mapper.Map<UserLocation>(locationDto);
            await _locationRepository.AddAsync(location);
        }


    }
}
