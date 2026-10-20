namespace ProductConfigurator.Api.Domain
{
    public class OptionGroup
    {
        public int Id { get; init; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; init; }
        public required string Name { get; set; }

        public SelectionMode Mode { get; set; }
        public ICollection<ConfigurationOption> ConfigurationOptions { get; set; } = [];
    }
}
