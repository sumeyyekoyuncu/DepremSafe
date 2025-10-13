using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;

namespace DepremSafe.Core.Interfaces
{
    public interface IUserLocationRepository
    {
        Task<UserLocation> GetByIdAsync(Guid id);
        Task<IEnumerable<UserLocation>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserLocation location);
    }
}
