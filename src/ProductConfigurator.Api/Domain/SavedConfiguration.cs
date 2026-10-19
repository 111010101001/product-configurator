namespace ProductConfigurator.Api.Domain
{
    public class SavedConfiguration
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTimeOffset Created { get; init; }
        public ICollection<OptionGroup> SelectedOptions { get; set; } = [];
    }
}
