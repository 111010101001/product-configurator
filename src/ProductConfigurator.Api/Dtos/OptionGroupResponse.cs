using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record OptionGroupResponse(int Id, string Name, SelectionMode Mode, new SelectedOptionResponse(int OptionId, string Code, string Name, string GroupName, decimal PriceDelta) Options);
}
