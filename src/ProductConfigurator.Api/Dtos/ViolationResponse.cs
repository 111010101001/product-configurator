namespace ProductConfigurator.Api.Dtos
{
    public record ViolationResponse(string Code, string Message, IReadOnlyList<int> OptionIds);
}
