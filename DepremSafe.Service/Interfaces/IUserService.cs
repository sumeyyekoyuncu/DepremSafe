using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;

namespace DepremSafe.Service.Interfaces
{
    public interface IUserService
    {

        //selam
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(Guid id);
        Task AddAsync(UserDTO userDto);
        Task UpdateAsync(UserDTO userDto);
        Task DeleteAsync(Guid id);
        Task<User?> GetByEmail(string email);
        Task<User> CreateGoogleUser(string email, string name, string googleId);
        Task UpdateCityAsync(Guid userId, string city);

    }
}
