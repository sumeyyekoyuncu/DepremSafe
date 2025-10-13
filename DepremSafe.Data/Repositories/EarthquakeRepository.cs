using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DepremSafe.Data.Repositories
{
    public class EarthquakeRepository:IEarthquakeRepository
    {
        private readonly DepremSafeDbContext _context;
        public EarthquakeRepository(DepremSafeDbContext context) => _context = context;
        public async Task<Earthquake> GetByIdAsync(Guid id) =>
           await _context.Earthquakes.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Earthquake>> GetAllAsync() =>
            await _context.Earthquakes.ToListAsync();

        public async Task AddAsync(Earthquake earthquake)
        {
            _context.Earthquakes.Add(earthquake);
            await _context.SaveChangesAsync();
        }
    }
}
