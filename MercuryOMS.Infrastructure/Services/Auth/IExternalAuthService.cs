using Microsoft.AspNetCore.Authentication;

namespace MercuryOMS.Infrastructure.Services
{
    public interface IExternalAuthService
    {
        AuthenticationProperties GetLoginProperties(string provider, string returnUrl);
    }
}
