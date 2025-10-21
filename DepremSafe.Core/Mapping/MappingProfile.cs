using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;

namespace DepremSafe.Core.Mapping
{
    public class MappingProfile : Profile

    {
        public MappingProfile()
        {
            CreateMap<Earthquake, EarthquakeDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserLocation, UserLocationDTO>().ReverseMap();
        }

    }
}
