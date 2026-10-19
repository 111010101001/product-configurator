namespace ProductConfigurator.Api.Domain
{
    public class CompatibilityRule
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public int Option1Id { get; init; }
        public int Option2Id { get; init; }
        public ConfigurationOption Option1 { get; set; }

        public ConfigurationOption Option2 { get; set; }

        public RuleType rule { get; set; }
    }
}
