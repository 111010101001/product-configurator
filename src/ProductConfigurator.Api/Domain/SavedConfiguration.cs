namespace ProductConfigurator.Api.Domain
{
    public class SavedConfiguration
    {
        public int Id { get; init; }
        public required string Name { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTimeOffset Created { get; init; }
        public ICollection<ConfigurationSelection> Selections { get; set; } = [];
    }
}
