using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Features;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMediator _mediator;
        private readonly IExternalAuthService _externalAuthService;
        private readonly string _frontendUrl;

        public AuthController(
            IConfiguration config,
            IMediator mediator,
            IExternalAuthService externalAuthService)
        {
            _config = config;
            _mediator = mediator;
            _externalAuthService = externalAuthService;
            _frontendUrl = config["App:FrontendUrl"]!; //BaseUrl
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

            if (!result.IsSuccess)
                return Unauthorized(result);

            var (accessToken, refreshToken) = result.Value;

            SetRefreshTokenCookie(refreshToken);

            return Ok(Result<string>.Success(accessToken, "Đăng nhập thành công"));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), ct);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _mediator.Send(new LogoutCommand(refreshToken), ct);
            }

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok();
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] ExternalProvider provider)
        {
            var props = _externalAuthService.GetLoginProperties(
                provider.ToString(),
                Url.Action(nameof(ExternalCallback), "Auth")!
            );

            return Challenge(props, provider.ToString());
        }

        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalCallback(CancellationToken ct)
        {
            var result = await _mediator.Send(new ExternalLoginCallbackCommand(), ct);

            if (!result.IsSuccess)
                return Redirect($"{_frontendUrl}/login?error=external_failed");

            var (_, refreshToken) = result.Value!;

            SetRefreshTokenCookie(refreshToken);

            return Redirect($"{_frontendUrl}/auth/callback");
            //return Ok(result.Value.accessToken);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, CancellationToken ct)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(userId, token), ct);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var days = int.Parse(_config["Jwt:RefreshTokenDays"]!);
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(days)
            });
        }
    }
}