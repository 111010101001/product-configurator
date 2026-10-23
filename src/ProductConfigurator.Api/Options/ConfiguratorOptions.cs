using System.ComponentModel.DataAnnotations;

namespace ProductConfigurator.Api.Options
{
    public class ConfiguratorOptions
    {
        public const string SectionName = "Configurator";
        [Range(1, 500)]
        public int MaxSelectedOptions { get; init; } = 50;
    }
}
