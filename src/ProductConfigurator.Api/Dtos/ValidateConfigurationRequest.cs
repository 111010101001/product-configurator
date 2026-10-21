namespace ProductConfigurator.Api.Dtos
{
    public record ValidateConfigurationRequest
    {
        public required List<int> OptionIds { get; init; }

    }
}
