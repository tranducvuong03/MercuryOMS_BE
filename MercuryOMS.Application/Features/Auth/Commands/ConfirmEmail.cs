using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>;

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IAuthService _authService;

        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmEmailAsync(request.UserId, request.Token);
        }
    }
}
