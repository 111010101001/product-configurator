using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Options;

namespace ProductConfigurator.Api.Services
{
    [ApiController]
    [Route("api")]
    public class ConfigurationsController(ConfigurationService configurations, IOptions<ConfiguratorOptions> options) : ControllerBase
    {
        [HttpPost("products/{productId:int}/configurations/validate")]
        public async Task<ActionResult<ValidationResultResponse>> Validate(int productId, ValidateConfigurationRequest request, CancellationToken ct)
        {
            if (!SelectionIsWellFormed(request.OptionIds))
                return ValidationProblem(ModelState);

            var result = await configurations.ValidateAsync(productId, request.OptionIds, ct);
            if (result is null)
                return NotFound();

            return result;
        }

        private bool SelectionIsWellFormed(List<int> optionIds)
        {
            var max = options.Value.MaxSelectedOptions;
            if (optionIds.Count > max)
                ModelState.AddModelError("optionIds", $"At most {max} options can be selected.");
            if (optionIds.Count != optionIds.Distinct().Count())
                ModelState.AddModelError("optionIds", "Each option can only be selected once.");
            return ModelState.IsValid;
        }
    }
}
