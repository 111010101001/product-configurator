using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public partial class ConfigurationService(ProductService products, 
        ConfigurationValidator validator,
        ConfiguratorDbContext db,
        ILogger<ProductService> logger,
        TimeProvider timeProvider)
    {
        public async Task<ValidationResultResponse?> ValidateAsync(int productId, IReadOnlyCollection<int> optionIds, CancellationToken ct)
        {
            var product = await products.GetByIdAsync(productId, ct);
            if (product is null)
                return null; // ProductService has already logged "not found"

            var result = validator.Validate(product, optionIds);
            LogProductConfiguration(productId, optionIds.Count, result.IsValid, result.Violations.Count);
            return result;
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

        [LoggerMessage(Level = LogLevel.Information, Message = "Validated configuration for product {ProductId}: {OptionCount} options, valid: {IsValid}, {ViolationCount} violations")]
        private partial void LogProductConfiguration(int productId, int optionCount, bool isValid, int violationCount);
    }
}
