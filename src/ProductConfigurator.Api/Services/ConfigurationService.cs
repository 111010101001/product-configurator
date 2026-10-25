using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ConfigurationService(ProductService products, ConfigurationValidator validator)
    {
        public async Task<ValidationResultResponse?> ValidateAsync(int productId, IReadOnlyCollection<int> optionIds, CancellationToken ct)
        {
            var product = await products.GetByIdAsync(productId, ct);
            return product is null ? null : validator.Validate(product, optionIds);
        }
    }
}
