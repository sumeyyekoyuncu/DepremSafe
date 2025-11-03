using DepremSafe.Core.DTOs;
using DepremSafe.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private IAiService _aiService;
        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// Starts a new chat session with the AI
        /// </summary>
        /// <param name="request">Contains userId and safety status</param>
        /// <returns>AI's initial message</returns>
        [HttpPost("start")]
        public async Task<IActionResult> StartChat([FromBody] StartChatRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { error = "UserId is required" });
            }

            try
            {
                var response = await _aiService.StartChatAsync(request.UserId, request.IsSafe);

                return Ok(new
                {
                    message = response,
                    userId = request.UserId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Continues the chat based on user's positive/negative feedback
        /// </summary>
        /// <param name="request">Contains userId and feedback</param>
        /// <returns>AI's follow-up message</returns>
        [HttpPost("continue")]
        public async Task<IActionResult> ContinueChat([FromBody] ContinueChatRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { error = "UserId is required" });
            }

            try
            {
                var response = await _aiService.ContinueChatAsync(request.UserId, request.IsPositive);

                return Ok(new
                {
                    message = response,
                    userId = request.UserId,
                    feedback = request.IsPositive ? "positive" : "negative",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the full chat history for a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>List of chat messages</returns>
        [HttpGet("history/{userId}")]
        public IActionResult GetChatHistory(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "UserId is required" });
            }

            try
            {
                var history = _aiService.GetChatHistory(userId);

                return Ok(new
                {
                    userId = userId,
                    messageCount = history.Count,
                    messages = history
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Clears chat history for a user (reset conversation)
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Success confirmation</returns>
        [HttpDelete("clear/{userId}")]
        public IActionResult ClearChat(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "UserId is required" });
            }

            try
            {
                _aiService.ClearChatHistory(userId);

                return Ok(new
                {
                    message = "Chat history cleared successfully",
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    
}
}
