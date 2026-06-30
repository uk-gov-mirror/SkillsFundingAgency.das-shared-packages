using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class FlagsEnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var clrType = context.JsonTypeInfo.Type;
        var enumType = clrType.IsEnum ? clrType
            : Nullable.GetUnderlyingType(clrType) is { IsEnum: true } u ? u : null;

        if (enumType == null || !enumType.IsDefined(typeof(FlagsAttribute), false))
            return Task.CompletedTask;

        schema.Type = JsonSchemaType.Integer;
        schema.Format = "int64";
        schema.Enum = Enum.GetValues(enumType)
            .Cast<object>()
            .Select(v => (JsonNode)JsonValue.Create((long)Convert.ChangeType(v, typeof(long)))!)
            .ToList();

        var namesArray = new JsonArray();
        foreach (var name in Enum.GetNames(enumType))
            namesArray.Add(JsonValue.Create(name));

        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-enumFlags"] = new JsonNodeExtension(JsonValue.Create(true)!);
        schema.Extensions["x-enumNames"] = new JsonNodeExtension(namesArray);
        schema.Description = "Flags enum — combine values with bitwise OR, or pass comma-separated names.";

        return Task.CompletedTask;
    }
}
