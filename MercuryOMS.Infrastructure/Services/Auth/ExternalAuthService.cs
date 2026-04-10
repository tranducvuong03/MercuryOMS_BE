using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace MercuryOMS.Infrastructure.Services
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalAuthService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public AuthenticationProperties GetLoginProperties(string provider, string returnUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        }
    }
}
