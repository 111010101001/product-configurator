using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record OptionGroupResponse(int Id, string Name, SelectionMode Mode, IReadOnlyList<OptionResponse> Options);
}
