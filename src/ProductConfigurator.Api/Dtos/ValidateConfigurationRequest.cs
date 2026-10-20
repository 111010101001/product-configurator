namespace ProductConfigurator.Api.Dtos
{
    public record ValidateConfigurationRequest
    {
        public required int OptionIds { get; set; }

    }
}
