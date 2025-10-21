using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DepremSafe.Service.Interfaces;
using Google.Apis.Auth.OAuth2;

namespace DepremSafe.Service.Services
{
    public class FcmService : IFcmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectId;
        private readonly GoogleCredential _googleCredential;

        public FcmService(HttpClient httpClient, string serviceAccountJsonPath)
        {
            _httpClient = httpClient;
            _googleCredential = GoogleCredential.FromFile(serviceAccountJsonPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            _projectId = JsonSerializer.Deserialize<JsonElement>(System.IO.File.ReadAllText(serviceAccountJsonPath))
                         .GetProperty("project_id").GetString();
        }

        public async Task SendNotificationAsync(string fcmToken, string title, string body)
        {
            // 1. Access token al
            var accessToken = await _googleCredential.UnderlyingCredential
                .GetAccessTokenForRequestAsync();

            // 2. HTTP v1 endpoint
            var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            // 3. Mesaj JSON
            var message = new
            {
                message = new
                {
                    token = fcmToken,
                    notification = new
                    {
                        title = title,
                        body = body
                    }
                }
            };

            var jsonMessage = JsonSerializer.Serialize(message);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}