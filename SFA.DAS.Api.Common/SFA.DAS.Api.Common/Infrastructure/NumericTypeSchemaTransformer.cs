using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class NumericTypeSchemaTransformer : IOpenApiSchemaTransformer
{
    // OpenAPI 3.1 emits "type": ["integer","string"] for numeric properties;
    // NSwag mishandles that combination and generates incorrect types.
    private const JsonSchemaType IntStrNull = JsonSchemaType.Integer | JsonSchemaType.String | JsonSchemaType.Null;
    private const JsonSchemaType IntStr = JsonSchemaType.Integer | JsonSchemaType.String;
    private const JsonSchemaType NumStrNull = JsonSchemaType.Number | JsonSchemaType.String | JsonSchemaType.Null;
    private const JsonSchemaType NumStr = JsonSchemaType.Number | JsonSchemaType.String;

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (!schema.Type.HasValue || schema.Enum is { Count: > 0 })
            return Task.CompletedTask;

        if (schema.Type == IntStrNull) schema.Type = JsonSchemaType.Integer | JsonSchemaType.Null;
        else if (schema.Type == IntStr) schema.Type = JsonSchemaType.Integer;
        else if (schema.Type == NumStrNull) schema.Type = JsonSchemaType.Number | JsonSchemaType.Null;
        else if (schema.Type == NumStr) schema.Type = JsonSchemaType.Number;

        return Task.CompletedTask;
    }
}
