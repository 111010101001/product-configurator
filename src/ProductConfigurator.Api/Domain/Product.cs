namespace ProductConfigurator.Api.Domain
{
    public class Product
    {
        public int Id { get; init; }

        public required string Name { get; set; }

        public required string Sku { get; set; }

        public ICollection<OptionGroup> OptionGroups { get; set; } = [];

        public ICollection<CompatibilityRule> CompatibilityRules { get; set; } = [];
        public decimal BasePrice { get; set; }
    }
}
