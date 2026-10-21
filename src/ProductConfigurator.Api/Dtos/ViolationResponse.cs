namespace ProductConfigurator.Api.Dtos
{
    public record ViolationResponse(bool IsValid, decimal TotalPrice);
}
