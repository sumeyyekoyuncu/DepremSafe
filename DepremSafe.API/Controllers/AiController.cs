using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private IAiService _aiService;
        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }
        [HttpGet("calm-message")]
        public async Task<IActionResult> GetCalmMessage(bool isSafe)
        {
            var message = await _aiService.GenerateCalmMessageAsync(isSafe);
            return Ok(new { message });
        }

    }
}
