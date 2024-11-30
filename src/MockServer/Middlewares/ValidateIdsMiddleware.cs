using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using MockServer.Attributes;
using Models.Contracts;
using static MockServer.Metas.Constants;

namespace MockServer.Middlewares;

public class ValidateIdsMiddleware : IQueryStringValidator
{
    private readonly RequestDelegate next;

    public ValidateIdsMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint() ?? throw new NullReferenceException();

        var qs = context.Request.Query;

        foreach (var q in qs)
        {
            if (q.Key == Q_PARAM_NAME_OMSID)
            {
                if (!Guid.TryParse(q.Value, out var validGuid))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    BadRequest br = new BadRequest();
                    br.globalErrors = new[] { $"Parameter '{Q_PARAM_NAME_OMSID}' from Query string has wrong format. Recived:{q.Value}. MUST be UUID" };
                    return context.Response.WriteAsync(JsonSerializer.Serialize(br));
                }
            }
        }

        var routeEndpoint = endpoint as RouteEndpoint;
        var actionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

        var rp = routeEndpoint.RoutePattern;

        // foreach (var param in rp.Parameters)
        {
            // Reflect on the action method and its parameters to get attributes
            if (actionDescriptor != null)
            {
                var parameters = actionDescriptor.MethodInfo.GetParameters();
                foreach (var parameter in parameters)
                {
                    // Get custom attributes of the parameter
                    var queryAtr = parameter.GetCustomAttribute<FromQueryAttribute>();
                    if (queryAtr != null)
                    {
                        var atrGuid = parameter.GetCustomAttribute<UUIDValidationAttribute>();
                        if (atrGuid != null)
                        {
                            var val = qs.First(x => x.Key == parameter.Name).Value;
                            if (!Guid.TryParse(val, out var validGuid))
                            {
                                BadRequest br = new BadRequest();
                                br.globalErrors = new[] { $"Parameter '{parameter.Name}' from Query string has wrong format. Recived:{val}. MUST be UUID" };
                                return context.Response.WriteAsync(JsonSerializer.Serialize(br));
                            }
                        }
                    }
                }
            }
        }
        return next.Invoke(context);
    }
}