namespace ProductConfigurator.Api.Domain
{
    public class CompatibilityRule
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public int SourceOptionId { get; init; }
        public int TargetOptionId { get; init; }
        public ConfigurationOption SourceOption { get; set; } = null!;

        public ConfigurationOption TargetOption { get; set; } = null!;

        public RuleType RuleType { get; set; }
    }
}
