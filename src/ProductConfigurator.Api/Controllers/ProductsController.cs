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
    }
}
