# Product Configurator API

All routes start with `/api`. Bodies are JSON with camelCase property names, which is the ASP.NET Core default. Enums are sent as strings. Errors are returned as ProblemDetails (`application/problem+json`), added in step 6.

## URL conventions

Nesting is shallow. A child resource is created under its parent, because it can't exist without one. After that it has its own id and is addressed directly, so no URL is more than one level deep:

- `POST /api/products/1/option-groups` creates a group in product 1
- `GET /api/option-groups/10` reads it

Option groups, options and rules have no list endpoints. All three are included in `GET /api/products/{productId}`, which is what a configurator UI loads anyway.

## Endpoints

### Products

| Method | Route | Request | Success | Errors |
|---|---|---|---|---|
| GET | `/api/products` | | 200 `ProductSummaryResponse[]` | |
| GET | `/api/products/{productId}` | | 200 `ProductDetailsResponse` | 404 |
| POST | `/api/products` | `ProductRequest` | 201 `ProductDetailsResponse` + Location | 400, 409 SKU in use |
| PUT | `/api/products/{productId}` | `ProductRequest` | 204 | 400, 404, 409 SKU in use |
| DELETE | `/api/products/{productId}` | | 204 | 404, 409 product has saved configurations |

### Option groups

| Method | Route | Request | Success | Errors |
|---|---|---|---|---|
| GET | `/api/option-groups/{groupId}` | | 200 `OptionGroupResponse` | 404 |
| POST | `/api/products/{productId}/option-groups` | `OptionGroupRequest` | 201 `OptionGroupResponse` + Location | 400, 404 product |
| PUT | `/api/option-groups/{groupId}` | `OptionGroupRequest` | 204 | 400, 404 |
| DELETE | `/api/option-groups/{groupId}` | | 204 | 404, 409 an option is used in a saved configuration |

### Options

| Method | Route | Request | Success | Errors |
|---|---|---|---|---|
| GET | `/api/options/{optionId}` | | 200 `OptionResponse` | 404 |
| POST | `/api/option-groups/{groupId}/options` | `OptionRequest` | 201 `OptionResponse` + Location | 400, 404 group, 409 code in use in this product |
| PUT | `/api/options/{optionId}` | `OptionRequest` | 204 | 400, 404, 409 code in use |
| DELETE | `/api/options/{optionId}` | | 204 | 404, 409 used in a saved configuration |

Deleting an option also deletes the rules that reference it. The service does that explicitly, because SQL Server won't cascade along both rule foreign keys (step 4).

### Compatibility rules

| Method | Route | Request | Success | Errors |
|---|---|---|---|---|
| GET | `/api/rules/{ruleId}` | | 200 `RuleResponse` | 404 |
| POST | `/api/products/{productId}/rules` | `RuleRequest` | 201 `RuleResponse` + Location | 400, 404 product, 409 duplicate rule, 422 option not in this product or source = target |
| DELETE | `/api/rules/{ruleId}` | | 204 | 404 |

There is no PUT. A rule is two option ids and a type, so changing it means deleting it and creating a new one.

### Saved configurations

| Method | Route | Request | Success | Errors |
|---|---|---|---|---|
| GET | `/api/products/{productId}/configurations` | | 200 `SavedConfigurationResponse[]` | 404 |
| GET | `/api/configurations/{configurationId}` | | 200 `SavedConfigurationResponse` | 404 |
| POST | `/api/products/{productId}/configurations` | `SaveConfigurationRequest` | 201 `SavedConfigurationResponse` + Location | 400, 404 product, 422 breaks a rule |
| DELETE | `/api/configurations/{configurationId}` | | 204 | 404 |
| POST | `/api/products/{productId}/configurations/validate` | `ValidateConfigurationRequest` | 200 `ValidationResultResponse` | 400, 404 product |

## Request shapes

| DTO | Fields |
|---|---|
| `ProductRequest` | `name`, `sku`, `basePrice` |
| `OptionGroupRequest` | `name`, `mode` (`ExactlyOne` / `AtMostOne` / `AnyNumber`) |
| `OptionRequest` | `code`, `name`, `priceDelta` |
| `RuleRequest` | `type` (`Requires` / `Excludes`), `sourceOptionId`, `targetOptionId` |
| `SaveConfigurationRequest` | `name`, `optionIds` |
| `ValidateConfigurationRequest` | `optionIds` |

No request contains an `id`, a `created` timestamp, or the parent id that is already in the URL.

## Response shapes

`ProductSummaryResponse`: `id`, `name`, `sku`, `basePrice`.

`ProductDetailsResponse`: the whole product as a tree, built from four entity types.

```json
{
  "id": 1,
  "name": "City Bike",
  "sku": "CB-2026",
  "basePrice": 8990.00,
  "optionGroups": [
    {
      "id": 10,
      "name": "Frame size",
      "mode": "ExactlyOne",
      "options": [
        { "id": 100, "code": "M", "name": "Medium frame", "priceDelta": 0.00 },
        { "id": 101, "code": "L", "name": "Large frame", "priceDelta": 0.00 }
      ]
    },
    {
      "id": 30,
      "name": "Extras",
      "mode": "AnyNumber",
      "options": [
        { "id": 301, "code": "SEAT", "name": "Child seat", "priceDelta": 1200.00 },
        { "id": 302, "code": "RACK", "name": "Rear rack", "priceDelta": 500.00 }
      ]
    }
  ],
  "rules": [
    { "id": 7, "type": "Requires", "sourceOptionId": 301, "targetOptionId": 302 }
  ]
}
```

`OptionGroupResponse`, `OptionResponse` and `RuleResponse` are the corresponding nested objects above. `OptionGroupResponse` includes its `options`.

`SavedConfigurationResponse`: combines SavedConfiguration, ConfigurationSelection, ConfigurationOption, OptionGroup and Product. The join entity doesn't appear, and `totalPrice` is computed.

```json
{
  "id": 42,
  "productId": 1,
  "name": "My commuter",
  "created": "2026-10-20T11:53:00+00:00",
  "selectedOptions": [
    { "optionId": 100, "code": "M", "name": "Medium frame", "groupName": "Frame size", "priceDelta": 0.00 },
    { "optionId": 302, "code": "RACK", "name": "Rear rack", "groupName": "Extras", "priceDelta": 500.00 }
  ],
  "totalPrice": 9490.00
}
```

`ValidationResultResponse`: corresponds to no entity.

```json
{
  "isValid": false,
  "totalPrice": 10190.00,
  "violations": [
    { "code": "MissingRequired", "message": "Child seat requires Rear rack.", "optionIds": [301, 302] },
    { "code": "SelectionCount", "message": "Frame size: select exactly one option.", "optionIds": [] }
  ]
}
```

Violation codes: `UnknownOption`, `SelectionCount`, `MissingRequired`, `Excluded`.

## Design decisions

**Validate is a POST, not a GET.** Validation changes nothing, so GET looks natural, but the input is a list of option ids. A GET body has no defined meaning in HTTP (RFC 9110), and many clients and proxies drop or reject it. A query string (`?optionIds=1&optionIds=5`) would work and could be cached, but the list can grow and the body then differs from the save request. So it is POST to a verb-named sub-resource, which is an accepted exception for actions that don't fit the resource model. An alternative some people prefer is `POST /validations`, treating the result as a resource. The HTTP `QUERY` method (a safe, idempotent method with a body) is an IETF proposal intended for exactly this case.

**Validating a broken configuration returns 200; saving one returns 422.** The validate request succeeded, and "this configuration is invalid" is its answer, so it goes in a 200 body. Saving is different: the client asked to store something the business rules forbid, so it returns 422 Unprocessable Content with the same violations inside the ProblemDetails. 400 is kept for input that is malformed: missing fields, wrong types, an empty name.

**404 is about the URL; ids in the body are validated.** `POST /api/products/99/rules` with an unknown product 99 is a 404. A `RuleRequest` whose `sourceOptionId` doesn't exist, or belongs to another product, is a 422: the resource in the URL was found, and the body is what's wrong.

**Price is recomputed on read.** `totalPrice` is `basePrice` plus the current `priceDelta` of each selected option. Price changes and discounts show up immediately, with no snapshot to keep in sync. The cost is that a saved configuration needs its options to keep existing, so deleting an option that a saved configuration uses returns 409. The alternative is to store `PriceDeltaAtSave` on the selection and show both prices. That suits quotes and orders, where a price a customer has seen must not change.

**Database constraint errors don't become 500s.** The service checks for conflicts (duplicate SKU, option in use) before writing, and returns 409 ProblemDetails. The database constraints stay as a backstop for races between two requests. A `DbUpdateException` from a unique index is translated to 409 rather than left to surface as a 500.

**Enums are strings on the wire.** `"ExactlyOne"` is readable in logs and in the OpenAPI document, and clients don't break if the numeric values change. A global `JsonStringEnumConverter` is registered in step 5. The database still stores integers.

**One request DTO for create and update.** `ProductRequest` serves both POST and PUT while the fields are the same. Split it into Create and Update versions when they diverge, for example if the SKU becomes immutable after creation.

**Integer ids in URLs.** They are sequential and easy to guess, so every endpoint must check that the caller may access that specific object (OWASP API Security #1, Broken Object Level Authorization). This API has no authentication. In a real API this would be the first thing to add.
