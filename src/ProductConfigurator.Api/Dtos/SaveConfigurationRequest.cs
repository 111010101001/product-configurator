namespace ProductConfigurator.Api.Dtos
{
    public record SaveConfigurationRequest
    {
        public required string Name { get; set; }

        public required List<int> OptionIds { get; init; }
    }
}
