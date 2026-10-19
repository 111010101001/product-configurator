namespace ProductConfigurator.Api.Domain
{
    public class ConfigurationSelection
    {
        public int SavedConfigurationId { get; init; }
        public int ConfigurationOptionId { get; init; }
        public SavedConfiguration Configuration { get; set; } = null!;
        public ConfigurationOption Option { get; set; } = null!;
    }
}
