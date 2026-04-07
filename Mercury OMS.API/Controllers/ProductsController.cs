using MediatR;
using MercuryOMS.Application.Features;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercuryOMS.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = Role.Seller)]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
        {
            var result = await _mediator.Send(
                command with { ProductId = id });

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Seller)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(
                new DeleteProductCommand(id));

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetProductByIdQuery(id));

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] ProductFilterRequest request)
        {
            var result = await _mediator.Send(
                new GetProducts(request));

            return Ok(result);
        }
    }
}