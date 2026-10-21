using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record OptionGroupRequest
    {
        public required string Name { get; init; }

        public required SelectionMode Mode { get; init; }
    }
}
