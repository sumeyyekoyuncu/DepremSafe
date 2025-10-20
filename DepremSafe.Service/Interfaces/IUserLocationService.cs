using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;

namespace DepremSafe.Service.Interfaces
{
    public interface IUserLocationService
    {
        Task<UserLocationDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<UserLocationDTO>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserLocationDTO locationDto);
       
    }
}
