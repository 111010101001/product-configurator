namespace ProductConfigurator.Api.Domain
{
    public class Product
    {
        public int Id { get; init; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public ICollection<OptionGroup> OptionGroups { get; set; } = [];

        public ICollection<CompatibilityRule> rules { get; set; } = [];
        public decimal BasePrice { get; set; }
    }
}
