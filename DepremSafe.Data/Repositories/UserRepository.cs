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
    public class UserRepository : IUserRepository
    {
        private readonly DepremSafeDbContext _context;
        public UserRepository(DepremSafeDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetByIdAsync(Guid id) =>
           await _context.Users.Include(u => u.Locations).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Locations).ToListAsync();

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
}
}
