using Microsoft.OpenApi.Models;
using MockServer.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MockServer;

public class StatusCode400Annotations : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo != null)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Все учтенные и обрабатываемые ошибки HTTP 4** имеют ту же схему",
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] =  new OpenApiMediaType()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(BadRequest), context.SchemaRepository)
                    }
                }
            });
        }
    }
}