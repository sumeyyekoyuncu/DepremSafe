using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;

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
    }
}
