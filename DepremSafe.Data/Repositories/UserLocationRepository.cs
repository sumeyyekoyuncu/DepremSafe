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
    public class UserLocationRepository:IUserLocationRepository
    {
        private readonly DepremSafeDbContext _context;
        public UserLocationRepository(DepremSafeDbContext context) => _context = context;

        public async Task<UserLocation> GetByIdAsync(Guid id) =>
            await _context.UserLocations.Include(l => l.User).FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<UserLocation>> GetByUserIdAsync(Guid userId) =>
            await _context.UserLocations.Where(l => l.UserId == userId).ToListAsync();

        public async Task AddAsync(UserLocation location)
        {
            _context.UserLocations.Add(location);
            await _context.SaveChangesAsync();
        }
    }
}
