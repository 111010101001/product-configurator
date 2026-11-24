using System.Diagnostics.Metrics;

namespace ProductConfigurator.Api.Telemetry
{
    public class ConfiguratorMetrics(IMeterFactory meterFactory)
    {
        private readonly Counter<long> validations = meterFactory
            .Create(ConfiguratorTelemetry.Name)
            .CreateCounter<long>("configurator.validations", unit: "{validation}",
                description: "Number of configuration validations");

        public void RecordValidation(bool isValid, string operation)
        {
            validations.Add(1, new KeyValuePair<string, object?>("configurator.valid", isValid), new KeyValuePair<string, object?>("configurator.operation", operation));
        }
    }
}
