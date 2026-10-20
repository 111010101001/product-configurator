namespace ProductConfigurator.Api.Domain
{
    public class CompatibilityRule
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public int Option1Id { get; init; }
        public int Option2Id { get; init; }
        public ConfigurationOption SourceOptionId { get; set; } = null!;

        public ConfigurationOption TargetOptionId { get; set; } = null!;

        public RuleType rule { get; set; }
    }
}
