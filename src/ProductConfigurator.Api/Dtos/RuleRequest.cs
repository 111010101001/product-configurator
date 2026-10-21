using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record RuleRequest
    {
        public required RuleType Type { get; init; }

        public required int SourceOptionId { get; init; }
        public required int TargetOptionId { get; init; }
    }
}
