using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Service.Interfaces;
using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;

namespace DepremSafe.Service.Services
{
    public class AuthService:IAuthService
    {
        private readonly FirebaseApp _firebaseApp;

        public AuthService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("depremsafe-firebasesdk.json")
                });
            }
            else
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
            }
        }

        public async Task<FirebaseToken> VerifyFirebaseTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Token doğrulanamadı: {ex.Message}");
            }
        
    }
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { "367485352063-33pbhnolq0jkgo1qqkj1fia4qpavfoi0.apps.googleusercontent.com" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }

    }
}
