namespace ProductConfigurator.Api.Dtos
{
    public record ProductRequest
    {
        public required string Name { get; init; }

        public required string Sku { get; init; }

        public required decimal BasePrice { get; init; }
    }
}
