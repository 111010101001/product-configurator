using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ConfigurationInvalidException(IReadOnlyList<ViolationResponse> violations)
        : Exception("The configuration breaks one or more product rules.")
    {
        public IReadOnlyList<ViolationResponse> Violations { get; } = violations;
    }
}
