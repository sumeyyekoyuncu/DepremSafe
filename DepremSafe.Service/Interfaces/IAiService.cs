using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;

namespace DepremSafe.Service.Interfaces
{
    public interface IAiService
    {
        Task<string> StartChatAsync(string userId, bool isSafe);
        Task<string> ContinueChatAsync(string userId, bool isPositiveResponse);
        void ClearChatHistory(string userId);
        List<ChatMessage> GetChatHistory(string userId);
    }
}
