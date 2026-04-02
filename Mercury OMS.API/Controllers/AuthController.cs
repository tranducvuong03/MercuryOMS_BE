using MediatR;
using MercuryOMS.Application.Features;
using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action("ExternalCallback", "Auth");

        var properties = _signInManager
                        .ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var result = await _mediator.Send(new ExternalLoginCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // dùng cho callback
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token, CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand(userId, token), ct);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
