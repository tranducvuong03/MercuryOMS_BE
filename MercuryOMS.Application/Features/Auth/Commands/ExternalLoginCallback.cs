using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record ExternalLoginCallbackCommand() : IRequest<Result<string>>;

    public class ExternalLoginCallbackHandler
    : IRequestHandler<ExternalLoginCallbackCommand, Result<string>>
    {
        private readonly IAuthService _authService;

        public ExternalLoginCallbackHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<string>> Handle(
            ExternalLoginCallbackCommand request,
            CancellationToken ct)
        {
            return await _authService.ExternalLoginCallbackAsync();
        }
    }
}
