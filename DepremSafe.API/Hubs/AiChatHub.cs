using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DepremSafe.API.Hubs
{
    public class AiChatHub:Hub
    {
        private readonly IAiService _aiService;
        public AiChatHub(IAiService aiService)
        {
            _aiService = aiService;
        }
        // Called when user first connects and starts chat
        public async Task StartChat(bool isSafe)
        {
            var userId = Context.ConnectionId;
            var response = await _aiService.StartChatAsync(userId, isSafe);

            await Clients.Caller.SendAsync("ReceiveMessage", response);
        }

        // Called when user responds (positive or negative)
        public async Task SendFeedback(bool isPositive)
        {
            var userId = Context.ConnectionId;
            var response = await _aiService.ContinueChatAsync(userId, isPositive);

            await Clients.Caller.SendAsync("ReceiveMessage", response);
        }

        // Get chat history
        public async Task GetHistory()
        {
            var userId = Context.ConnectionId;
            var history = _aiService.GetChatHistory(userId);

            await Clients.Caller.SendAsync("ReceiveHistory", history);
        }

        // Clear chat when user disconnects or resets
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var userId = Context.ConnectionId;
            _aiService.ClearChatHistory(userId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task ResetChat()
        {
            var userId = Context.ConnectionId;
            _aiService.ClearChatHistory(userId);

            await Clients.Caller.SendAsync("ChatReset");
        }
    }
}
