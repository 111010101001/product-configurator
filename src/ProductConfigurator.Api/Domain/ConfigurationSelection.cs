namespace ProductConfigurator.Api.Domain
{
    public class ConfigurationSelection
    {
        public int SavedConfigurationId { get; init; }
        public int ConfigurationOptionId { get; init; }
        public SavedConfiguration SavedConfiguration { get; set; } = null!;
        public ConfigurationOption ConfigurationOption { get; set; } = null!;
    }
}
