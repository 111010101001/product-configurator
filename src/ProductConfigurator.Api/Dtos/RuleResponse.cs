using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record RuleResponse(int Id, RuleType Rule, int SourceOptionId, int TargetOptionId);
}
