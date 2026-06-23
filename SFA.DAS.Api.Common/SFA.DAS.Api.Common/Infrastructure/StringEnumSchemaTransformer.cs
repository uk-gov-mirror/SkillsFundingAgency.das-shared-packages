using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class StringEnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var clrType = context.JsonTypeInfo.Type;
        var enumType = clrType.IsEnum ? clrType
            : Nullable.GetUnderlyingType(clrType) is { IsEnum: true } u ? u : null;

        if (enumType == null
            || enumType.IsDefined(typeof(FlagsAttribute), false)
            || schema.Enum is not { Count: > 0 }
            || schema.Type != null)
            return Task.CompletedTask;

        schema.Type = JsonSchemaType.String;
        // This is to fix an issue with OpenAPI 3.1 nullable types  and how they surface as null in the component schema's enum list.
        // NSwag generates a wrapper class instead of resolving $ref when null is present —
        // nullability is expressed at the property level via oneOf, not in the referenced schema.
        // this is required for the contract generation from the open api spec
        schema.Enum = schema.Enum
            .Where(v => v != null && v.GetValueKind() != JsonValueKind.Null)
            .ToList();

        return Task.CompletedTask;
    }
}
