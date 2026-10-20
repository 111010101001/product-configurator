using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record OptionGroupResponse(int id, string name, SelectionMode mode, IReadOnlyList<ConfigurationOption> optionGroups);
}
