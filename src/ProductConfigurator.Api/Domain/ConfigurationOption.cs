namespace ProductConfigurator.Api.Domain
{
    public class ConfigurationOption
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Code { get; set; }
        public OptionGroup OptionGroup { get; set; } = null!;
        public int OptionGroupId { get; set; }
        public decimal PriceDelta { get; set; }
        public int CompatibilityRuleId { get; init; }


    }
}
