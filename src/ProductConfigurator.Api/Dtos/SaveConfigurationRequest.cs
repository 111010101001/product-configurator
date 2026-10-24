using System.ComponentModel.DataAnnotations;

namespace ProductConfigurator.Api.Dtos
{
    public record SaveConfigurationRequest
    {
        [Required, StringLength(50)]
        public required string Name { get; init; }

        public required List<int> OptionIds { get; init; }
    }
}
