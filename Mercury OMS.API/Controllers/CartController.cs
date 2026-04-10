using MediatR;
using MercuryOMS.Application.Features;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/carts")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        var result = await _mediator.Send(new RemoveFromCartCommand(productId));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}