namespace ProductConfigurator.Api.Dtos
{
    public record OptionRequest
    {
        public required string Name { get; set; }

        public required string Code { get; set; }

        public decimal PriceDelta { get; set; }
    }
}
