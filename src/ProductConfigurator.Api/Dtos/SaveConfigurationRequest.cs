namespace ProductConfigurator.Api.Dtos
{
    public record SaveConfigurationRequest
    {
        public required string Name { get; set; }

        public required int OptionIds { get; set; }
    }
}
