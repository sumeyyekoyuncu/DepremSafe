using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DepremSafe.Service.Interfaces;

namespace DepremSafe.Service.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "7yzE79bGebwbBnOkngk3t5RlpowWMlr1py1dp2dB";

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateCalmMessageAsync(bool isSafe)
        {
            var prompt = isSafe
                ? "Kullanıcı depremi atlattı ve şu anda güvende. Ona kısa, sade ve moral verici bir telkin yaz. Türkçe konuş."
                : "Kullanıcı enkaz altında veya korkmuş durumda. Onu sakinleştir, umut ver ve kısa konuş. Türkçe yaz.";

            var body = new
            {
                model = "command-a-03-2025",
                message = prompt,
                temperature = 0.7,
                max_tokens = 100
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            // ✅ Yeni endpoint
            var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/chat", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Hata: {response.StatusCode} - {json}";

            using var doc = JsonDocument.Parse(json);

            // ✅ Cohere Chat yanıtı genelde {"text": "..."} döner
            if (doc.RootElement.TryGetProperty("text", out var textProp))
                return textProp.GetString()?.Trim() ?? "Sakin ol, yardım yolda.";

            // Alternatif yapı kontrolü
            if (doc.RootElement.TryGetProperty("message", out var msgProp))
                return msgProp.GetString()?.Trim() ?? "Sakin ol, yardım yolda.";

            return "Beklenmeyen yanıt alındı.";
        }
    
}
}

