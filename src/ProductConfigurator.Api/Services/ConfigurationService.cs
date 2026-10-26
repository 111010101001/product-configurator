using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ConfigurationService(ProductService products, 
        ConfigurationValidator validator,
        ConfiguratorDbContext db,
        TimeProvider timeProvider)
    {
        public async Task<ValidationResultResponse?> ValidateAsync(int productId, IReadOnlyCollection<int> optionIds, CancellationToken ct)
        {
            var product = await products.GetByIdAsync(productId, ct);
            return product is null ? null : validator.Validate(product, optionIds);
        }

        public async Task<SavedConfigurationResponse?> GetByIdAsync(int configurationId, CancellationToken ct)
        {
            return await db.SavedConfigurations.Where(x => x.Id == configurationId).Select(x => new SavedConfigurationResponse(x.Id, x.ProductId, x.Name, x.Created, x.Selections.OrderBy(s => s.ConfigurationOptionId).Select(s => new SelectedOptionResponse(s.ConfigurationOptionId, s.ConfigurationOption.Code, s.ConfigurationOption.Name, s.ConfigurationOption.OptionGroup.Name, s.ConfigurationOption.PriceDelta)).ToList(), x.Product.BasePrice + x.Selections.Sum(q => q.ConfigurationOption.PriceDelta))).FirstOrDefaultAsync(ct);
        }

        public async Task<SavedConfigurationResponse?> SaveAsync(int productId, SaveConfigurationRequest request, CancellationToken ct)
        {
            var validation = await ValidateAsync(productId, request.OptionIds.ToArray(), ct);
            if (validation is null)
                return null;
            if (validation.IsValid == false)
            {
                throw new ConfigurationInvalidException(validation.Violations);
            }
            var configuration = new SavedConfiguration()
            {
                Name = request.Name,
                Created = timeProvider.GetUtcNow(),
                ProductId = productId,
                Selections = request.OptionIds.Select(x => new ConfigurationSelection() { ConfigurationOptionId = x }).ToList()
            };
            db.SavedConfigurations.Add(configuration);
            await db.SaveChangesAsync(ct);
            return await GetByIdAsync(configuration.Id, ct);
        }
    }
}
