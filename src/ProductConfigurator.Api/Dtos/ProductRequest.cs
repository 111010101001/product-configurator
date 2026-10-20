namespace ProductConfigurator.Api.Dtos
{
    public record ProductRequest
    {
        public required string Name { get; set; }

        public required string Sku { get; set; }

        public required decimal BasePrice { get; set; }
    }
}
