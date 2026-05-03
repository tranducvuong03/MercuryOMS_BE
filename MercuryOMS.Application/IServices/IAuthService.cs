using MercuryOMS.Application.Commons;

namespace MercuryOMS.Application.IServices
{
    public interface IAuthService
    {
        Task<Result<(string accessToken, string refreshToken)>> LoginAsync(
            string email,
            string password,
            CancellationToken ct);

        Task<Result<(string accessToken, string refreshToken)>> ExternalLoginCallbackAsync(
            CancellationToken ct);

        Task<Result<string>> RefreshAsync(
            string refreshToken,
            CancellationToken ct);

        Task<Result> LogoutAsync(
            string refreshToken,
            CancellationToken ct);

        Task<Result> RegisterAsync(
            string email,
            string password,
            string fullName,
            CancellationToken ct);

        Task<Result> ConfirmEmailAsync(
            string userId,
            string token);

        Task<Result> ResendConfirmEmailAsync(
            string email,
            CancellationToken ct);
    }
}