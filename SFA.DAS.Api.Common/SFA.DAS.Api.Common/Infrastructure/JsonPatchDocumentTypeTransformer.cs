using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class JsonPatchDocumentTypeTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (!string.Equals(context.Description.HttpMethod, "patch", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var bodyParam = context.Description.ParameterDescriptions
            .FirstOrDefault(p => p.Source.Id == "Body");

        if (bodyParam?.Type is not { IsGenericType: true } paramType ||
            !typeof(JsonPatchDocument<>).IsAssignableFrom(paramType.GetGenericTypeDefinition()))
        {
            return Task.CompletedTask;
        }

        var innerType = paramType.GetGenericArguments().FirstOrDefault();
        if (innerType == null)
        {
            return Task.CompletedTask;
        }
        
        operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        operation.Extensions["x-patch-document-type"] = new JsonNodeExtension(JsonValue.Create(innerType.Name)!);

        return Task.CompletedTask;
    }
}
