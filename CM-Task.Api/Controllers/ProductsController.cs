using CM_Task.Application.Products.Commands.CreateProduct;
using CM_Task.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CM_Task.Api.Controllers;

public sealed class ProductsController(IMediator mediator) : BaseController
{
    [HttpGet]
    [EnableRateLimiting("api")]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducts(CancellationToken ct)
    {
        return Ok(await mediator.Send(new GetProductsQuery(), ct));
    }

    [HttpPost]
    [EnableRateLimiting("api")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var productId = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAllProducts), new { id = productId }, productId);
    }
}