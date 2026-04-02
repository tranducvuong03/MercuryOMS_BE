using MercuryOMS.Application.Commons;

namespace MercuryOMS.Application.IServices
{
    public interface IAuthService
    {
        Task<Result<string>> LoginAsync(string email, string password, CancellationToken ct);
        Task<Result> RegisterAsync(string email, string password, string fullName, CancellationToken ct);
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task<Result> ResendConfirmEmailAsync(string email, CancellationToken ct);
    }
}
