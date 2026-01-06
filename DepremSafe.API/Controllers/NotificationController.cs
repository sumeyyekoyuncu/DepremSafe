using DepremSafe.Core.DTOs;
using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using DepremSafe.Service.Interfaces;
using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly DepremSafeDbContext _context;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            DepremSafeDbContext context,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _context = context;
            _logger = logger;
        }

        // FCM Token güncelleme
        [HttpPost("update-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FcmToken))
                {
                    return BadRequest("Token boş olamaz");
                }

                // GEÇİCİ: Email olmadan test için - son kullanıcıyı güncelle
                var lastUser = await _context.Users
                    .OrderByDescending(u => u.Id)
                    .FirstOrDefaultAsync();

                if (lastUser == null)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }

                lastUser.FcmToken = request.FcmToken;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Token güncellendi: {lastUser.Email}");

                return Ok(new
                {
                    message = "Token güncellendi",
                    userEmail = lastUser.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token güncelleme hatası: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("check-tokens")]
        public async Task<IActionResult> CheckTokens()
        {
            try
            {
                var usersWithTokens = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.FcmToken))
                    .Select(u => new
                    {
                        u.Email,
                        u.FcmToken,
                        TokenPreview = u.FcmToken.Substring(0, Math.Min(30, u.FcmToken.Length)) + "..."
                    })
                    .ToListAsync();

                return Ok(new
                {
                    count = usersWithTokens.Count,
                    users = usersWithTokens
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token kontrolü hatası: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // Deprem uyarısı gönder
        [HttpPost("earthquake-alert")]
        public async Task<IActionResult> SendEarthquakeAlert([FromBody] EarthquakeAlertRequest request)
        {
            try
            {
                // Bölgedeki tüm kullanıcıların token'larını al
                var tokens = await _context.Users
                    .Where(u => u.City == request.City && !string.IsNullOrEmpty(u.FcmToken))
                    .Select(u => u.FcmToken)
                    .ToListAsync();

                if (!tokens.Any())
                {
                    return Ok(new { message = "Bildirim gönderilecek kullanıcı bulunamadı", count = 0 });
                }

                var success = await _notificationService.SendEarthquakeAlert(tokens, request.EarthquakeData);

                return Ok(new
                {
                    message = success ? "Bildirimler gönderildi" : "Bildirim gönderilemedi",
                    count = tokens.Count,
                    success = success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Deprem uyarısı hatası: {ex.Message}");
                return StatusCode(500, "Bildirim gönderilirken hata oluştu");
            }
        }

        // Yardım isteği bildirimi
        [HttpPost("help-request")]
        public async Task<IActionResult> NotifyNearbyUsers([FromBody] HelpNotificationRequest request)
        {
            try
            {
                // Yakındaki kullanıcıları bul (10km yarıçap)
                var nearbyTokens = await GetNearbyUserTokens(
                    request.Latitude,
                    request.Longitude,
                    radiusKm: 10
                );

                if (!nearbyTokens.Any())
                {
                    return Ok(new { message = "Yakında kullanıcı bulunamadı", count = 0 });
                }

                var success = await _notificationService.SendHelpRequest(nearbyTokens, request);

                return Ok(new
                {
                    message = "Yardım bildirimi gönderildi",
                    count = nearbyTokens.Count,
                    success = success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Yardım bildirimi hatası: {ex.Message}");
                return StatusCode(500, "Bildirim gönderilirken hata oluştu");
            }
        }

        // Test bildirimi gönder
        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification([FromBody] SendTestNotificationRequest request)
        {
            try
            {
                // Test için ilk kullanıcıya gönder
                var user = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.FcmToken))
                    .FirstOrDefaultAsync();

                if (user == null || string.IsNullOrEmpty(user.FcmToken))
                {
                    return NotFound("Token'lı kullanıcı bulunamadı");
                }

                var success = await _notificationService.SendTestNotification(
                    user.FcmToken,
                    request.Title,
                    request.Body,
                    request.Type
                );

                return Ok(new
                {
                    message = success ? "Test bildirimi gönderildi" : "Bildirim gönderilemedi",
                    success = success,
                    sentTo = user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Test bildirimi hatası: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // Yakındaki kullanıcıları bul (yardımcı metod)
        private async Task<List<string>> GetNearbyUserTokens(double latitude, double longitude, double radiusKm)
        {
            // Basit yakınlık hesabı (gerçek uygulamada Haversine formülü kullanın)
            var allUsers = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.FcmToken))
                .ToListAsync();

            // TODO: Kullanıcıların location bilgisi varsa gerçek mesafe hesabı yapın
            // Şimdilik tüm kullanıcıları döndürüyoruz
            return allUsers.Select(u => u.FcmToken).ToList();
        }
    

}
}
