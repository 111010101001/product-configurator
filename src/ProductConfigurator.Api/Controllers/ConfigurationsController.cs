using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Options;
using ProductConfigurator.Api.Services;

namespace ProductConfigurator.Api.Controllers
{
    [Produces("application/json")]
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

        [ProducesResponseType<SavedConfigurationResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [HttpGet("configurations/{configurationId:int}")]
        public async Task<ActionResult<SavedConfigurationResponse>> GetById(int configurationId, CancellationToken ct)
        {
            var config = await configurations.GetByIdAsync(configurationId, ct);
            if (config is null)
                return NotFound();

            return config;
        }

        [ProducesResponseType<SavedConfigurationResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [HttpPost("products/{productId:int}/configurations")]
        public async Task<ActionResult<SavedConfigurationResponse>> Save(int productId, SaveConfigurationRequest request, CancellationToken ct)
        {
            if (!SelectionIsWellFormed(request.OptionIds))
                return ValidationProblem(ModelState);
            var result = await configurations.SaveAsync(productId, request, ct);
            if (result is null)
                return NotFound();
            return CreatedAtAction(nameof(GetById), new { configurationId = result.Id }, result);
        }
    }
}
