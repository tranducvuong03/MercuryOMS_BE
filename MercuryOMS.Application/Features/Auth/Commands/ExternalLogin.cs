using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record ExternalLoginCommand() : IRequest<Result<string>>;

    public class ExternalLoginHandler : IRequestHandler<ExternalLoginCommand, Result<string>>
    {
        private readonly IAuthService _authService;

        public ExternalLoginHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<string>> Handle(ExternalLoginCommand request, CancellationToken ct)
        {
            return await _authService.ExternalLoginAsync();
        }
    }
}
