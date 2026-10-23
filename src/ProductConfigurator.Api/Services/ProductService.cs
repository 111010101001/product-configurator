using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ProductService
    {
        private readonly ConfiguratorDbContext configuratorDbContext;
        private ILogger<ProductService> _logger;
        public ProductService(ConfiguratorDbContext configuratorDbContext, ILogger<ProductService> logger)
        {
            this.configuratorDbContext = configuratorDbContext;
            _logger = logger;
        }

        public async Task<IReadOnlyList<ProductSummaryResponse>> GetAllAsync(CancellationToken ct)
        {
            return await configuratorDbContext.Products
                .OrderBy(x => x.Name)
                .Select(x => new ProductSummaryResponse(x.Id,x.Name,x.Sku,x.BasePrice))
                .ToListAsync(ct);
        }

        public async Task<ProductDetailsResponse?> GetByIdAsync(int id, CancellationToken ct) 
        {
            var product = await configuratorDbContext.Products
                .Where(x => x.Id == id)
                .Select(x => new ProductDetailsResponse(
                    x.Id, 
                    x.Name, 
                    x.Sku, 
                    x.BasePrice, 
                    x.OptionGroups
                        .OrderBy(g => g.Id)
                        .Select(g => new OptionGroupResponse( 
                            g.Id, 
                            g.Name, 
                            g.Mode,
                            g.ConfigurationOptions
                                .OrderBy(c => c.Id)
                                .Select(c => new OptionResponse(c.Id, c.Code, c.Name, c.PriceDelta))
                                .ToList())).ToList(), 
                    x.CompatibilityRules.OrderBy(c => c.Id)
                    .Select(r => new RuleResponse(r.Id, r.RuleType, r.SourceOptionId, r.TargetOptionId)).ToList()))
                .AsSplitQuery().FirstOrDefaultAsync(ct);

            if (product is null) 
            {
                _logger.LogInformation("Product {ProductId} not found", id);
            }
            return product;
        }
    }
}
