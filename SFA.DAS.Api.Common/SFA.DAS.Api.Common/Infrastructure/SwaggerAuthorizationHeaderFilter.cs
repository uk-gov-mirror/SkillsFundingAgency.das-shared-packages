using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class AuthorizationHeaderTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IOpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                AllowEmptyValue = false,
                Example = JsonValue.Create("Bearer [KEY]"),
                Required = true
            });

            return Task.CompletedTask;
        }
    }
}
