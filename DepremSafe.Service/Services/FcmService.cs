using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DepremSafe.Service.Interfaces;

namespace DepremSafe.Service.Services
{
    public class FcmService:IFcmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverKey; // Firebase konsolundan aldığın Server Key

        public FcmService(HttpClient httpClient, string serverKey)
        {
            _httpClient = httpClient;
            _serverKey = serverKey;
        }

        public async Task SendNotificationAsync(string fcmToken, string title, string body)
        {
            var message = new
            {
                to = fcmToken,
                notification = new
                {
                    title = title,
                    body = body
                }
            };

            var jsonMessage = JsonSerializer.Serialize(message);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("key", "=" + _serverKey);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
