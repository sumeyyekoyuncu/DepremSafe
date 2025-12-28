using DepremSafe.Service.Interfaces;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Mvc;
using DepremSafe.Core.DTOs;
using FirebaseAdmin.Auth;
using DepremSafe.Service.Services;

namespace DepremSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService,IUserService userService,IJwtService jwtService)
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
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

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await _authService.VerifyGoogleToken(request.IdToken);

            var email = payload.Email;
            var name = payload.Name;
            var googleId = payload.Subject;

            // Kullanıcı var mı?
            var user = await _userService.GetByEmail(email);

            if (user == null)
            {
                user = await _userService.CreateGoogleUser(email, name, googleId);
            }

            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    IsSafe = (bool)user.IsSafe
                }
            };

            return Ok(response);
        }

        }
    }
