using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Services;

namespace ProductConfigurator.Tests
{
    public class ConfigurationValidatorTests(ApiFactory factory) : IClassFixture<ApiFactory>
    {
        private static OptionResponse Option(int id, string name) => new(id, name, name, 0m);

        private static OptionGroupResponse Group(string name, params OptionResponse[] options)
            => new(0, name, SelectionMode.ExactlyOne, options);
        private static readonly ProductDetailsResponse Bike = new(1, "Bike", "BIKE", 0m,
            [Group("Frame size", Option(1, "Small frame"), Option(2, "Medium frame")),
             Group("Wheels",     Option(3, "Small wheels"), Option(4, "Large wheels"))],
            [new RuleResponse(1, RuleType.Requires, 1, 3)]
            );

        private static readonly JsonSerializerOptions Json =
            new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

        [Theory]
        [InlineData(new[] { 2, 4 }, true)]
        [InlineData(new[] { 1, 3 }, true)]
        [InlineData(new[] { 1, 4 }, false)]
        public void Requires_rule_applies_only_when_its_source_is_selected(int[] selectedIds, bool expectedValid)
        {
            var validator = new ConfigurationValidator();

            Assert.Equal(expectedValid, validator.Validate(Bike, selectedIds).IsValid );
        }

        [Fact]
        public async Task Saving_a_configuration_that_breaks_a_rule_returns_422()
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
            var products = await client.GetAsync("/api/products");
            List <ProductSummaryResponse> product = await products.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(Json);
            Assert.NotNull(product);
            var cityBike = product.FirstOrDefault(x => x.Name == "City Bike");
            Assert.NotNull(cityBike);
            var productDetailsJson = await client.GetAsync($"/api/products/{cityBike.Id}");
            ProductDetailsResponse productDetails = await productDetailsJson.Content.ReadFromJsonAsync<ProductDetailsResponse>(Json);
            Assert.NotNull(productDetails);

            var smallFrame = productDetails.OptionGroups.SelectMany(g => g.Options).First(o => o.Code == "S");

            var largeWheels = productDetails.OptionGroups.SelectMany(g => g.Options).First(o => o.Code == "W-L");

            var request = new SaveConfigurationRequest { Name = "Broken", OptionIds = [smallFrame.Id, largeWheels.Id] };
            var response = await client.PostAsJsonAsync($"/api/products/{cityBike.Id}/configurations", request);

            Assert.Equal(HttpStatusCode.UnprocessableContent, response.StatusCode);
        }
    }
}
