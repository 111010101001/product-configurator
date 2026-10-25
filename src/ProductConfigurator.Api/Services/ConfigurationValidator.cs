using ProductConfigurator.Api.Domain;
using ProductConfigurator.Api.Dtos;

namespace ProductConfigurator.Api.Services
{
    public class ConfigurationValidator
    {
        public ValidationResultResponse Validate(ProductDetailsResponse product, IReadOnlyCollection<int> selectedOptionIds)
        {
            var violations = new List<ViolationResponse>();

            // Every option of the product, looked up by id, together with its group
            var options = product.OptionGroups
                .SelectMany(g => g.Options, (group, option) => (Group: group, Option: option))
                .ToDictionary(x => x.Option.Id);

            var selected = selectedOptionIds.ToHashSet();

            // 1. Unknown options
            foreach (var id in selected.Where(id => !options.ContainsKey(id)))
                violations.Add(new ViolationResponse("UnknownOption", $"Option {id} doesn't belong to this product.", [id]));

            // 2. Selection count per group  → "SelectionCount"
            foreach (OptionGroupResponse optionGroup in product.OptionGroups)
            {
                var mode = optionGroup.Mode;
                var numberOfInstances = optionGroup.Options.Select(x => x.Id).Count(x => selected.Contains(x));
                var ids = optionGroup.Options.Select(x => x.Id).Where(x => selected.Contains(x));
                switch (mode)
                {
                    case SelectionMode.AtMostOne:
                        if (numberOfInstances > 1)
                        {
                            violations.Add(new ViolationResponse("SelectionCount", $"Option group cant be combined {optionGroup.Name}.", ids.ToArray()));
                        }
                        break;
                    case SelectionMode.ExactlyOne:
                        if (numberOfInstances != 1)
                        {
                            violations.Add(new ViolationResponse("SelectionCount", $"Option group must be 1 {optionGroup.Name}.", ids.ToArray()));
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var rule in product.Rules)
            {
                var type = rule.Type;
                switch (type) 
                {
                    case RuleType.Excludes:
                        if (selected.Contains(rule.SourceOptionId) && selected.Contains(rule.TargetOptionId))
                        {
                            violations.Add(new ViolationResponse("Excluded", $"Cant combine Options {options[rule.SourceOptionId].Option.Name} {options[rule.TargetOptionId].Option.Name}.", [ rule.SourceOptionId, rule.TargetOptionId ]));
                        }
                        break;
                    case RuleType.Requires:
                        if (selected.Contains(rule.SourceOptionId) && !selected.Contains(rule.TargetOptionId))
                        {
                            violations.Add(new ViolationResponse("MissingRequired", $"Must combine Options {options[rule.SourceOptionId].Option.Name} {options[rule.TargetOptionId].Option.Name}.", [ rule.SourceOptionId, rule.TargetOptionId ]));
                        }
                        break;
                    default : break;
                }
            }


            var totalPrice = product.BasePrice + selected.Where(options.ContainsKey).Sum(id => options[id].Option.PriceDelta);
            return new ValidationResultResponse(violations.Count == 0, totalPrice, violations);
        }
    }
}
