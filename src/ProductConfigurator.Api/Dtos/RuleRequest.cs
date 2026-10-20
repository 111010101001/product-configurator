namespace ProductConfigurator.Api.Dtos
{
    public record RuleRequest
    {
        public required string Type { get; set; }

        public required int SourceOptionId { get; set; }
        public required int TargetOptionId { get; set; }
    }
}
