using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;

namespace DepremSafe.Core.Interfaces
{
    public interface IEarthquakeRepository
    {
        Task<Earthquake> GetByIdAsync(Guid id);
        Task<IEnumerable<Earthquake>> GetAllAsync();
        Task AddAsync(Earthquake earthquake);
    }
}
