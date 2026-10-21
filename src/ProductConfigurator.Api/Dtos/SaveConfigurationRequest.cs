namespace ProductConfigurator.Api.Dtos
{
    public record SaveConfigurationRequest
    {
        public required string Name { get; init; }

        public required List<int> OptionIds { get; init; }
    }
}
