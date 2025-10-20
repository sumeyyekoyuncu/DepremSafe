using DepremSafe.Core.DTOs;
using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepremSafe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EarthquakesController : ControllerBase
    {
        private readonly IEarthquakeService _earthquakeService;

        public EarthquakesController(IEarthquakeService earthquakeService)
        {
            _earthquakeService = earthquakeService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EarthquakeDTO>>> GetAll()
        {
            var earthquakes = await _earthquakeService.GetAllAsync();
            return Ok(earthquakes);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EarthquakeDTO>> GetById(Guid id)
        {
            var earthquake = await _earthquakeService.GetByIdAsync(id);
            if (earthquake == null) return NotFound();
            return Ok(earthquake);
        }
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] EarthquakeDTO earthquakeDto)
        {
            await _earthquakeService.AddAsync(earthquakeDto);
            return CreatedAtAction(nameof(GetById), new { id = earthquakeDto.Id }, earthquakeDto);
        }


    }
}
