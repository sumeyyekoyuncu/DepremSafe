using DepremSafe.Core.DTOs;
using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepremSafe.API.Controllers
{
    public class UserLocationsController : ControllerBase
    {
        private readonly IUserLocationService _locationService;
        public UserLocationsController(IUserLocationService locationService)
        {
            _locationService = locationService;
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<UserLocationDTO>> GetById(Guid id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null) return NotFound();
            return Ok(location);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserLocationDTO>>> GetByUserId(Guid userId)
        {
            var locations = await _locationService.GetByUserIdAsync(userId);
            return Ok(locations);
        }
       




    }
}
