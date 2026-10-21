using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record SavedConfigurationResponse(int Id, string Name, DateTimeOffset Created, IReadOnlyList<OptionResponse> SelectedOptions, decimal totalPrice);
}
