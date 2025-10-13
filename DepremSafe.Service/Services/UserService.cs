using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;

namespace DepremSafe.Service.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository ) { _userRepository = userRepository; }
        public Task<User> GetByIdAsync(Guid id) => _userRepository.GetByIdAsync(id);
        public Task<IEnumerable<User>> GetAllAsync() => _userRepository.GetAllAsync();
        public Task AddAsync(User user) => _userRepository.AddAsync(user);
        public Task UpdateAsync(User user) => _userRepository.UpdateAsync(user);
        public Task DeleteAsync(Guid id) => _userRepository.DeleteAsync(id);
    }
}
