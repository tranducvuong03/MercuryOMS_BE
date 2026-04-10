using MercuryOMS.Infrastructure.Identity;

namespace MercuryOMS.Application.IServices
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
