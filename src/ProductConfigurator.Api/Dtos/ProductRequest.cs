namespace ProductConfigurator.Api.Dtos
{
    public record ProductRequest
    {
        public required string Name { get; set; }

        public required string Sku { get; set; }

        public decimal BasePrice { get; set; }
    }
}
