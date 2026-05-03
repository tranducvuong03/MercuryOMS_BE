using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record LoginCommand(string Email, string Password)
        : IRequest<Result<(string accessToken, string refreshToken)>>;

    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, Result<(string accessToken, string refreshToken)>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<(string accessToken, string refreshToken)>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(
                request.Email,
                request.Password,
                cancellationToken);
        }
    }
}