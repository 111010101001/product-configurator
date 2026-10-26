using Microsoft.AspNetCore.Mvc;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Services;

namespace ProductConfigurator.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/products")]
    public class ProductsController(ProductService products) : ControllerBase
    {
        [HttpGet]
        public async Task<IReadOnlyList<ProductSummaryResponse>> GetAll(CancellationToken ct) 
        {
            return await products.GetAllAsync(ct);
        }

        [ProducesResponseType<ProductDetailsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ProductDetailsResponse>> GetById(int productId, CancellationToken ct) 
        {
            var product = await products.GetByIdAsync(productId, ct);
            if (product is null)
                return NotFound();

            return product;
        }

        [ProducesResponseType<ProductDetailsResponse>(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult<ProductDetailsResponse>> Create(ProductRequest request, CancellationToken ct)
        {
            var created = await products.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { productId = created.Id }, created);
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [HttpPut("{productId:int}")]
        public async Task<IActionResult> Update(int productId, ProductRequest request, CancellationToken ct)
        {
            var result = await products.UpdateAsync(productId, request, ct);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Delete(int productId, CancellationToken ct)
        {
            var result = await products.DeleteAsync(productId, ct);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
