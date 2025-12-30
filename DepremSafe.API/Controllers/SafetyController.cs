using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;
using DepremSafe.Data.Context;
using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class SafetyController : ControllerBase
    {
        private readonly DepremSafeDbContext _context;
        private readonly ILogger<SafetyController> _logger;

        public SafetyController(
            DepremSafeDbContext context,
            ILogger<SafetyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("safety-status")]
        public async Task<IActionResult> ReportSafetyStatus([FromBody] SafetyStatusRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId))
                {
                    return BadRequest(new { message = "UserId gerekli" });
                }

                var safetyReport = new SafetyReport
                {
                    UserId = request.UserId,
                    IsSafe = request.IsSafe,
                    Latitude = request.Location?.Latitude,
                    Longitude = request.Location?.Longitude,
                    Accuracy = request.Location?.Accuracy,
                    ReportedAt = DateTime.UtcNow
                };

                await _context.SafetyReports.AddAsync(safetyReport);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Güvenlik durumu kaydedildi: UserId={UserId}, IsSafe={IsSafe}, Location={Location}",
                    request.UserId,
                    request.IsSafe,
                    request.Location != null ? $"({request.Location.Latitude}, {request.Location.Longitude})" : "Yok"
                );

                // Güvenli değilse acil durum işlemleri
                if (!request.IsSafe && request.Location != null)
                {
                    // TODO: Acil durum ekiplerine bildirim gönder
                    _logger.LogWarning(
                        "ACİL DURUM: UserId={UserId}, Konum=({Lat}, {Lng})",
                        request.UserId,
                        request.Location.Latitude,
                        request.Location.Longitude
                    );
                }

                return Ok(new { message = "Güvenlik durumu başarıyla kaydedildi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Güvenlik durumu kaydedilirken hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası" });
            }
        }

        // Opsiyonel: Güvenli olmayan kullanıcıları listele
        [HttpGet("unsafe-users")]
        public async Task<IActionResult> GetUnsafeUsers()
        {
            try
            {
                var unsafeReports = await _context.SafetyReports
                    .Where(r => !r.IsSafe && r.ReportedAt > DateTime.UtcNow.AddHours(-24))
                    .OrderByDescending(r => r.ReportedAt)
                    .Select(r => new
                    {
                        r.UserId,
                        r.Latitude,
                        r.Longitude,
                        r.ReportedAt,
                        r.City
                    })
                    .ToListAsync();

                return Ok(unsafeReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Güvenli olmayan kullanıcılar listelenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası" });
            }
        }

        // Opsiyonel: Kullanıcı güvenlik geçmişi
        [HttpGet("user-history/{userId}")]
        public async Task<IActionResult> GetUserSafetyHistory(string userId)
        {
            try
            {
                var history = await _context.SafetyReports
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.ReportedAt)
                    .Take(10)
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı geçmişi alınırken hata");
                return StatusCode(500, new { message = "Sunucu hatası" });
            }
        }
    }
}