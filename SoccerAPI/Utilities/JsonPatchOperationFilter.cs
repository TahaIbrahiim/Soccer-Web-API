using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace SoccerAPI.Configuration 
{
    public class JsonPatchOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null)
            {
                var content = operation.RequestBody.Content;

                if (content.ContainsKey("application/json-patch+json"))
                {
                    var schema = context.SchemaGenerator.GenerateSchema(
                        typeof(JsonPatchDocument<>).MakeGenericType(context.MethodInfo.GetParameters().First().ParameterType),
                        context.SchemaRepository
                    );

                    content["application/json-patch+json"].Schema = schema;
                }
            }
        }
    }
}
