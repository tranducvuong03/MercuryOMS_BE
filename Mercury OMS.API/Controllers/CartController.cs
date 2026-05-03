using MediatR;
using MercuryOMS.Application.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/carts")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartQuery(), ct);

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("items")]
    public async Task<IActionResult> RemoveFromCart(
        [FromQuery] Guid productId,
        [FromQuery] Guid variantId)
    {
        var result = await _mediator.Send(
            new RemoveFromCartCommand(productId, variantId));

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}