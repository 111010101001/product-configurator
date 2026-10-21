namespace ProductConfigurator.Api.Dtos
{
    public record ValidationResultResponse(bool IsValid, decimal TotalPrice, IReadOnlyList<ViolationResponse> ViolationResponses);
}
