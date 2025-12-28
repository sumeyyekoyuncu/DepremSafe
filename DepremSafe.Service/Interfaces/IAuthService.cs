using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;

namespace DepremSafe.Service.Interfaces
{
    public interface IAuthService
    {
        Task<FirebaseToken> VerifyFirebaseTokenAsync(string idToken);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken);
    }
}
