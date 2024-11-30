using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Build.Evaluation;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MockServer.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MockServer;

public class SwaggerTryItOutDefaultValueFilter : ISchemaFilter
{
    /// <summary>
    /// Apply is called for each parameter
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.ParameterInfo != null)
        {
            var att = context.ParameterInfo.GetCustomAttribute<SwaggerTryItOutDefaultValueAttribute>();
            if (att != null)
            {
                var type = att.Value.GetType();
                if (type == typeof(string))
                {
                    schema.Example = new OpenApiString((string)att.Value);
                }
                else if (type == typeof(int))
                {
                    schema.Example = new OpenApiInteger((int)att.Value);
                }
                else
                {
                    throw new NotImplementedException(type.AssemblyQualifiedName);
                }
            }
        }
    }
}