using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record LogoutCommand(string RefreshToken)
        : IRequest<Result>;

    public class LogoutCommandHandler
        : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(
            LogoutCommand request,
            CancellationToken ct)
        {
            return await _authService.LogoutAsync(request.RefreshToken, ct);
        }
    }
}