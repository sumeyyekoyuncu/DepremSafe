using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using DepremSafe.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DepremSafe.Service.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly DepremSafeDbContext _context;
        public UserService(IUserRepository userRepository, IMapper mapper,DepremSafeDbContext context)
        {
            _userRepository = userRepository;
           _mapper = mapper;
            _context = context;
        }
        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }
        public async Task<UserDTO?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }
        public async Task AddAsync(UserDTO userDto)
        {
            // User entity'ye map et
            var user = _mapper.Map<User>(userDto);

            // Burada navigation property kullanarak otomatik lokasyon ekle
            var location = new UserLocation
            {
                City = user.City,      // User'ın şehir bilgisini al
                Latitude = 0.0,        // varsayılan veya başlangıç değerleri
                Longitude = 0.0,
                Source = "Default",    // otomatik kaynak
                User = user            // navigation property ile ilişkilendir
            };

            user.Locations = new List<UserLocation> { location };

            // User ve ilişkili UserLocation aynı anda eklenir
            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _userRepository.DeleteAsync(id);
        }
        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }
       
       public async Task<User> CreateGoogleUser(string email, string name, string googleId)
        {
            var user = new User
            {
                Email = email,
                Username = name,
                GoogleId = googleId,
                LoginProvider = "Google",
                IsSafe = true,
                LastUpdated = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task UpdateCityAsync(Guid userId, string city)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            user.City = city;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


    }
}
