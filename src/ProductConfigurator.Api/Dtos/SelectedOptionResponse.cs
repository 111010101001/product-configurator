namespace ProductConfigurator.Api.Dtos
{
    public record SelectedOptionResponse(int OptionId, string Code, string Name, string GroupName, decimal PriceDelta);
}
