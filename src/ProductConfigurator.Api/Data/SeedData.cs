using ProductConfigurator.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace ProductConfigurator.Api.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ConfiguratorDbContext db, CancellationToken ct = default)
        {
            // Return straight away if any product already exists
            if (await db.Products.AnyAsync(ct))
            {
                return;
            }

            // Build the product as one object graph with object initializers
            var largeFrame = new ConfigurationOption { Code = "L", Name = "Large frame", PriceDelta = 300m };
            var mediumFrame = new ConfigurationOption { Code = "M", Name = "Medium frame", PriceDelta = 200m };
            var smallFrame = new ConfigurationOption { Code = "S", Name = "Small frame", PriceDelta = 100m };
            var smallLights = new ConfigurationOption { Code = "HL-S", Name = "Small headlights", PriceDelta = 10m };
            var largeLights = new ConfigurationOption { Code = "HL-L", Name = "Large headlights", PriceDelta = 30m };

            var smallWheels = new ConfigurationOption { Code = "W-S", Name = "Small Wheels", PriceDelta = 50m };
            var largeWheels = new ConfigurationOption { Code = "W-L", Name = "Large Wheels", PriceDelta = 100m };

            var bell = new ConfigurationOption { Code = "B", Name = "Bell", PriceDelta = 10m };
            var kickstand = new ConfigurationOption { Code = "K", Name = "Kickstand", PriceDelta = 10m };


            var bike = new Product
            {
                Name = "City Bike",
                Sku = "CB-2026",
                BasePrice = 8990m,
                OptionGroups =
                [
                    new OptionGroup
                        {
                            Name = "Frame size",
                            Mode = SelectionMode.ExactlyOne,
                            ConfigurationOptions =
                            [
                                smallFrame,
                                mediumFrame,
                                largeFrame,
                            ]
                        },
                    new OptionGroup
                        {
                            Name = "Headlights",
                            Mode = SelectionMode.AtMostOne,
                            ConfigurationOptions =
                            [
                                smallLights,
                                largeLights,
                            ]
                        },
                    new OptionGroup
                        {
                            Name = "Wheels",
                            Mode = SelectionMode.ExactlyOne,
                            ConfigurationOptions = [ smallWheels, largeWheels ]
                        },
                    new OptionGroup
                        {
                            Name = "Extras",
                            Mode = SelectionMode.AnyNumber,
                            ConfigurationOptions = [bell, kickstand]
                        },
                ],
                CompatibilityRules =
                [

                ]

            };
            db.Products.Add(bike);

            await db.SaveChangesAsync(ct);   // real ids exist from here on

            bike.CompatibilityRules.Add(new CompatibilityRule
            {
                RuleType = RuleType.Excludes,
                SourceOptionId = Math.Min(smallFrame.Id, largeLights.Id),
                TargetOptionId = Math.Max(smallFrame.Id, largeLights.Id),
            });

            bike.CompatibilityRules.Add(new CompatibilityRule
            {
                RuleType = RuleType.Requires,
                SourceOptionId = smallFrame.Id,     // the option that has the requirement
                TargetOptionId = smallWheels.Id,    // the option it requires
            });

            await db.SaveChangesAsync(ct);
        }
    }
}
