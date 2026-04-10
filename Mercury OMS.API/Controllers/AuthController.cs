using MediatR;
using MercuryOMS.Application.Features;
using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IExternalAuthService _externalAuthService;

        public AuthController(IMediator mediator, IExternalAuthService externalAuthService)
        {
            _mediator = mediator;
            _externalAuthService = externalAuthService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery]ExternalProvider provider)
        {
            var props = _externalAuthService.GetLoginProperties(
                provider.ToString(),
                Url.Action("ExternalCallback")!
            );

            return Challenge(props, provider.ToString());
        }

        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalCallback()
        {
            var result = await _mediator.Send(new ExternalLoginCallbackCommand());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, CancellationToken ct)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(userId, token), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}