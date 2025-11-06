using DepremSafe.Service.Interfaces;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Mvc;
using DepremSafe.Core.DTOs;
using FirebaseAdmin.Auth;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] Core.DTOs.TokenRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.IdToken))
                return BadRequest("Token boş olamaz.");

            try
            {
                FirebaseToken token = await _authService.VerifyFirebaseTokenAsync(model.IdToken);

                return Ok(new
                {
                    uid = token.Uid,
                    issuer = token.Issuer,
                    claims = token.Claims
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
    }
