using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record OptionGroupRequest
    {
        public required string Name { get; set; }

        public required SelectionMode Mode { get; set; }
    }
}
