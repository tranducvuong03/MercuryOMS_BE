using MediatR;
using MercuryOMS.Application.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/sellers")]
    public class SellerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SellerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("become-seller")]
        public async Task<IActionResult> CreateSeller(
            [FromBody] CreateSellerCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToSeller(
            [FromBody] AddProductToSellerCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}