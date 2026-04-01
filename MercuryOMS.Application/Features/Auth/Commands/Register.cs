using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.FullName,
                cancellationToken);
        }
    }
}