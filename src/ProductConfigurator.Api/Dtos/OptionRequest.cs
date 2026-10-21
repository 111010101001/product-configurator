namespace ProductConfigurator.Api.Dtos
{
    public record OptionRequest
    {
        public required string Name { get; init; }

        public required string Code { get; init; }

        public decimal PriceDelta { get; init; }
    }
}
