using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace DepremSafe.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendEarthquakeAlert(List<string> tokens, EarthquakeData earthquakeData)
        {
            try
            {
                if (tokens == null || !tokens.Any())
                {
                    _logger.LogWarning("Token listesi boş");
                    return false;
                }

                var message = new MulticastMessage()
                {
                    Tokens = tokens,
                    Data = new Dictionary<string, string>()
                    {
                        { "type", "earthquake_alert" },
                        { "title", "⚠️ Deprem Uyarısı" },
                        { "body", "Bölgenizde deprem tespit edildi!" },
                        { "magnitude", earthquakeData.Magnitude.ToString("F1") },
                        { "location", earthquakeData.Location },
                        { "depth", earthquakeData.Depth.ToString("F1") },
                        { "time", earthquakeData.Time.ToString("O") },
                        { "latitude", earthquakeData.Latitude.ToString() },
                        { "longitude", earthquakeData.Longitude.ToString() }
                    },
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            Title = "⚠️ Deprem Uyarısı",
                            Body = $"Büyüklük: {earthquakeData.Magnitude} - {earthquakeData.Location}",
                            Sound = "default",
                            ChannelId = "earthquake_alerts"
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                _logger.LogInformation($"Başarıyla gönderilen: {response.SuccessCount}, Başarısız: {response.FailureCount}");

                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Deprem bildirimi gönderme hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendHelpRequest(List<string> nearbyUserTokens, HelpNotificationRequest data)
        {
            try
            {
                if (nearbyUserTokens == null || !nearbyUserTokens.Any())
                {
                    _logger.LogWarning("Yakında kullanıcı bulunamadı");
                    return false;
                }

                var message = new MulticastMessage()
                {
                    Tokens = nearbyUserTokens,
                    Data = new Dictionary<string, string>()
                    {
                        { "type", "help_request" },
                        { "title", "🆘 Yardım İsteği" },
                        { "body", "Yakınınızda yardıma ihtiyacı olan biri var" },
                        { "distance", data.Distance.ToString("F1") },
                        { "userId", data.UserId },
                        { "latitude", data.Latitude.ToString() },
                        { "longitude", data.Longitude.ToString() }
                    },
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            Title = "🆘 Yardım İsteği",
                            Body = $"Yakınında ({data.Distance:F1} km) yardıma ihtiyacı olan biri var",
                            ChannelId = "help_requests"
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                _logger.LogInformation($"Yardım bildirimi gönderildi: {response.SuccessCount} başarılı");

                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Yardım bildirimi hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmergencyUpdate(List<string> tokens, string title, string body)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = tokens,
                    Data = new Dictionary<string, string>()
                    {
                        { "type", "emergency_update" },
                        { "title", title },
                        { "body", body }
                    },
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            Title = title,
                            Body = body,
                            ChannelId = "emergency_updates"
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Acil durum bildirimi hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendTestNotification(string token, string title, string body, string type)
        {
            try
            {
                var message = new Message()
                {
                    Token = token,
                    Data = new Dictionary<string, string>()
                    {
                        { "type", type },
                        { "title", title },
                        { "body", body }
                    },
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            Title = title,
                            Body = body
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Test bildirimi gönderildi: {response}");

                return !string.IsNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Test bildirimi hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<int> SendToMultipleDevices(List<string> tokens, Dictionary<string, string> data)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = tokens,
                    Data = data
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
                return response.SuccessCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Toplu bildirim hatası: {ex.Message}");
                return 0;
            }
        }
    }
}
