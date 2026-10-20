using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record RuleRequest
    {
        public required RuleType Type { get; set; }

        public required int SourceOptionId { get; set; }
        public required int TargetOptionId { get; set; }
    }
}
