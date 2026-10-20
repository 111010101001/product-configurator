namespace ProductConfigurator.Api.Domain
{
    public class ConfigurationOption
    {
        public int Id { get; init; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public OptionGroup OptionGroup { get; set; } = null!;
        public int OptionGroupId { get; set; }
        public decimal PriceDelta { get; set; }

    }
}
