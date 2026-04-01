using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record ResendConfirmEmailCommand(string Email) : IRequest<Result>;

    public class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand, Result>
    {
        private readonly IAuthService _authService;

        public ResendConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResendConfirmEmailAsync(request.Email, cancellationToken);
        }
    }
}
