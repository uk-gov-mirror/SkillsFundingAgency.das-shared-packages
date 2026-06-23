using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure;

public class HealthChecksTransformer : IOpenApiDocumentTransformer
{
    private const string HealthCheckEndpoint = "/health";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var healthyResponse = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Enum = [JsonValue.Create("Healthy")]
                    }
                }
            }
        };

        var unhealthyResponse = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Enum = [JsonValue.Create("Unhealthy")]
                    }
                }
            }
        };

        var operation = new OpenApiOperation
        {
            Tags = new HashSet<OpenApiTagReference> { new OpenApiTagReference("Service Status", document) },
            Responses = new OpenApiResponses
            {
                ["200"] = healthyResponse,
                ["503"] = unhealthyResponse
            }
        };

        var pathItem = new OpenApiPathItem();
        pathItem.AddOperation(HttpMethod.Get, operation);

        document.Paths ??= new OpenApiPaths();
        document.Paths.Add(HealthCheckEndpoint, pathItem);

        return Task.CompletedTask;
    }
}
