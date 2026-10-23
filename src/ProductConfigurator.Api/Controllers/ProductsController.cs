using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Services;

namespace ProductConfigurator.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController(ProductService products) : ControllerBase
    {
        [HttpGet]
        public async Task<IReadOnlyList<ProductSummaryResponse>> GetAll(CancellationToken ct) 
        {
            return await products.GetAllAsync(ct);
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ProductDetailsResponse>> GetById(int productId, CancellationToken ct) 
        {
            var product = await products.GetByIdAsync(productId, ct);
            if (product is null)
                return NotFound();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailsResponse>> Create(ProductRequest request, CancellationToken ct)
        {
            var created = await products.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { productId = created.Id }, created);
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> Update(int productId, ProductRequest request, CancellationToken ct)
        {
            var result = await products.UpdateAsync(productId, request, ct);
            if (!result)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Delete(int productId, CancellationToken ct)
        {
            var result = await products.DeleteAsync(productId, ct);
            if (!result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
