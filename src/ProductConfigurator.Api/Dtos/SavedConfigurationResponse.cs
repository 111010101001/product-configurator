using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record SavedConfigurationResponse(int Id, int ProductId, string Name, DateTimeOffset Created, IReadOnlyList<SelectedOptionResponse> SelectedOptions, decimal TotalPrice);
}
