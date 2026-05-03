using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record RefreshTokenCommand(string RefreshToken)
        : IRequest<Result<string>>;

    public class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, Result<string>>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<string>> Handle(
            RefreshTokenCommand request,
            CancellationToken ct)
        {
            return await _authService.RefreshAsync(request.RefreshToken, ct);
        }
    }
}