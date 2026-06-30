using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class VersionHeaderTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IOpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Version",
                In = ParameterLocation.Header,
                AllowEmptyValue = false,
                Example = JsonValue.Create("1.0"),
                Required = true
            });

            return Task.CompletedTask;
        }
    }
}
