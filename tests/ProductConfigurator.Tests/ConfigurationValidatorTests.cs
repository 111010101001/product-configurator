using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core;
using Microsoft.Extensions.Options;
using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;
using ProductConfigurator.Api.Services;

namespace ProductConfigurator.Tests
{
    public class ConfigurationValidatorTests()
    {
        private static OptionResponse Option(int id, string name) => new(id, name, name, 0m);

        private static OptionGroupResponse Group(string name, params OptionResponse[] options)
            => new(0, name, SelectionMode.ExactlyOne, options);
        private static readonly ProductDetailsResponse Bike = new(1, "Bike", "BIKE", 0m,
            [Group("Frame size", Option(1, "Small frame"), Option(2, "Medium frame")),
             Group("Wheels",     Option(3, "Small wheels"), Option(4, "Large wheels"))],
            [new RuleResponse(1, RuleType.Requires, 1, 3)]
            );
        [Theory]
        [InlineData(new[] { 2, 4 }, true)]
        [InlineData(new[] { 1, 3 }, true)]
        [InlineData(new[] { 1, 4 }, false)]
        public void Requires_rule_applies_only_when_its_source_is_selected(int[] selectedIds, bool expectedValid)
        {
            var validator = new ConfigurationValidator();

            Assert.Equal(expectedValid, validator.Validate(Bike, selectedIds).IsValid );
        }
    }
}
