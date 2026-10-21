using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record ProductDetailsResponse(int Id, string Name, string Sku, decimal BasePrice, IReadOnlyList<OptionGroupResponse> OptionGroups, IReadOnlyList<RuleResponse> Rules);

}
