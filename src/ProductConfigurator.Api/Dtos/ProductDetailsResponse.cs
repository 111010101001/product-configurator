using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record ProductDetailsResponse(int id, string name, string sku, decimal BasePrice, IReadOnlyList<OptionGroup> optionGroups);

}
