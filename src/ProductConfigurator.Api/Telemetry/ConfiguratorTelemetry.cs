using System.Diagnostics;

namespace ProductConfigurator.Api.Telemetry
{
    public static class ConfiguratorTelemetry
    {
        public const string Name = "ProductConfigurator";
        public static readonly ActivitySource Source = new(Name) ;
    }
}
