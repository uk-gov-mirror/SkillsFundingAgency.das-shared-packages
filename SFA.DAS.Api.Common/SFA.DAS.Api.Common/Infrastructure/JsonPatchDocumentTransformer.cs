using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class JsonPatchDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (document.Components?.Schemas is not { } schemas)
        {
            return Task.CompletedTask;
        }

        var key = schemas.Keys.FirstOrDefault(k => k.Equals("operation", StringComparison.OrdinalIgnoreCase));
        if (key != null && schemas.TryGetValue(key, out var schema) && schema is OpenApiSchema openApiSchema)
            openApiSchema.Properties?.Remove("operationType");

        return Task.CompletedTask;
    }
}
