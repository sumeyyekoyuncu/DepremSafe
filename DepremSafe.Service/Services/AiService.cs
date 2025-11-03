using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;
using DepremSafe.Service.Interfaces;

namespace DepremSafe.Service.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "7yzE79bGebwbBnOkngk3t5RlpowWMlr1py1dp2dB";

        // Store chat history per user session
        private static readonly Dictionary<string, List<ChatMessage>> _chatHistories = new();

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> StartChatAsync(string userId, bool isSafe)
        {
            // Initialize new chat session
            _chatHistories[userId] = new List<ChatMessage>();

            var initialPrompt = isSafe
                ? "Kullanıcı depremi atlattı ve şu anda güvende. Ona kısa, sade ve moral verici bir telkin yaz. Sonrasında ona nasıl hissettiğini sor. Türkçe konuş."
                : "Kullanıcı enkaz altında veya korkmuş durumda. Onu sakinleştir, umut ver ve kısa konuş. Sonra ona durumu hakkında sor. Türkçe yaz.";

            return await SendMessageAsync(userId, initialPrompt, true);
        }

        public async Task<string> ContinueChatAsync(string userId, bool isPositiveResponse)
        {
            if (!_chatHistories.ContainsKey(userId) || !_chatHistories[userId].Any())
            {
                return "Sohbet başlatılmamış. Lütfen önce StartChatAsync çağırın.";
            }

            // Build context-aware prompt based on user's positive/negative response
            var userFeedback = isPositiveResponse ? "pozitif" : "negatif";

            var contextPrompt = $"Kullanıcı son mesajına {userFeedback} yanıt verdi. " +
                $"Önceki konuşmayı dikkate alarak, empati kur ve kısa bir sonraki mesajı yaz. " +
                $"Kullanıcıya başka bir soru sor veya destek ol. Türkçe konuş.";

            return await SendMessageAsync(userId, contextPrompt, false);
        }

        private async Task<string> SendMessageAsync(string userId, string prompt, bool isInitial)
        {
            // Get chat history for context
            var history = _chatHistories.GetValueOrDefault(userId) ?? new List<ChatMessage>();

            // Build chat history for Cohere API
            var chatHistory = history.Select(h => new
            {
                role = h.Role,
                message = h.Content
            }).ToList();

            var body = new
            {
                model = "command-a-03-2025",
                message = prompt,
                chat_history = chatHistory,
                temperature = 0.7,
                max_tokens = 150,
                preamble = "Sen empatik, destekleyici bir deprem kriz asistanısın. Kısa ve öz konuşursun."
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/chat", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Hata: {response.StatusCode} - {json}";

            using var doc = JsonDocument.Parse(json);

            string aiResponse = "Sakin ol, yardım yolda.";

            if (doc.RootElement.TryGetProperty("text", out var textProp))
                aiResponse = textProp.GetString()?.Trim() ?? aiResponse;
            else if (doc.RootElement.TryGetProperty("message", out var msgProp))
                aiResponse = msgProp.GetString()?.Trim() ?? aiResponse;

            // Save to chat history
            if (!isInitial)
            {
                _chatHistories[userId].Add(new ChatMessage
                {
                    Role = "USER",
                    Content = prompt
                });
            }

            _chatHistories[userId].Add(new ChatMessage
            {
                Role = "CHATBOT",
                Content = aiResponse
            });

            return aiResponse;
        }

        public void ClearChatHistory(string userId)
        {
            if (_chatHistories.ContainsKey(userId))
            {
                _chatHistories.Remove(userId);
            }
        }

        public List<ChatMessage> GetChatHistory(string userId)
        {
            return _chatHistories.GetValueOrDefault(userId) ?? new List<ChatMessage>();
        }
    }
}


