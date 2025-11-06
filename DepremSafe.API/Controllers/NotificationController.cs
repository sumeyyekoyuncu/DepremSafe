using DepremSafe.Core.DTOs;
using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IFcmService _fcmService;

        public NotificationController(IFcmService fcmService)
        {
            _fcmService = fcmService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationRequest request)
        {
            await _fcmService.SendNotificationAsync(request.Token, request.Title, request.Body);
            return Ok("Bildirim gönderildi");
        }
    
}
}
