// =============================================================================
// FakeAuthHeaderFilter.cs
// =============================================================================
// This is a Swagger/OpenAPI filter that automatically adds an "X-User-Id"
// header input field to every endpoint in the Swagger UI.
//
// Without this, you'd have to manually type the header name and value each
// time you test an endpoint. With this, every endpoint shows a pre-labeled
// input box where you just type 1, 2, or 3.
//
// HOW IT WORKS:
// Swashbuckle (the Swagger library) calls this filter for every endpoint
// it discovers. We add a header parameter to each one.
// =============================================================================

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RolesAssignments.Middleware;

public class FakeAuthHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add the X-User-Id header parameter to every endpoint
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-User-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Fake user ID: 1 = Ms. Rivera (Teacher), 2 = Mr. Daniels (Paraeducator), 3 = Dr. Patel (Supervisor)",
            Schema = new OpenApiSchema { Type = "integer" }
        });
    }
}
