namespace ProductConfigurator.Api.Domain
{
    public class OptionGroup
    {
        public int Id { get; init; }
        public int ConfigurationSelectionId { get; set; }
        public int SavedConfigurationId { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; init; }
        public string Name { get; set; }

        public SelectionMode Mode { get; set; }
        public ICollection<ConfigurationOption> ConfigurationOptions { get; set; } = [];
    }
}
