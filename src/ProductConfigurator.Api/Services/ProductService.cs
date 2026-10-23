using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ProductService
    {
        ConfiguratorDbContext configuratorDbContext;
        public ProductService(ConfiguratorDbContext configuratorDbContext)
        {
            this.configuratorDbContext = configuratorDbContext;
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
            return await configuratorDbContext.Products
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
        }
    }
}
