using System.ComponentModel.DataAnnotations;

namespace ProductConfigurator.Api.Dtos
{
    public record ProductRequest
    {
        [Required, StringLength(50)]
        public required string Name { get; init; }

        [Required, StringLength(50), RegularExpression("^[A-Z0-9-]+$")]
        public required string Sku { get; init; }

        [Range(typeof(decimal), "0", "1000000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public  decimal BasePrice { get; init; }
    }
}
