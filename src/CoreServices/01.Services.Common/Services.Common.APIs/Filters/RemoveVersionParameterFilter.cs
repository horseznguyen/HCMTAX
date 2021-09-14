using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Services.Common.APIs.Filters
{
    public class RemoveVersionParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version");

            if (versionParameter == default(OpenApiParameter)) return;

            var parameters = operation.Parameters.ToList();

            operation.Parameters.Remove(versionParameter);

            //var versionParameter = operation.Parameters.Single(p => p.Name == "version");

            //operation.Parameters.Remove(versionParameter);
        }
    }
}