using ProductConfigurator.Api.Domain;

namespace ProductConfigurator.Api.Dtos
{
    public record RuleResponse(int Id, RuleType Type, int SourceOptionId, int TargetOptionId);
}
