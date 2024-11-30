using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.IISIntegration;
using MockServer.Attributes;
using Models.Contracts;

namespace MockServer.Middlewares;

public class ValidateNotDefault : IBodyValidator
{
    private readonly RequestDelegate next;

    public ValidateNotDefault(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>
    /// Работает только для самого верхнего (root) объекта в иерархии, который десиреализуется 
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NullReferenceException"></exception>
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint() ?? throw new NullReferenceException();

        var routeEndpoint = endpoint as RouteEndpoint;
        var actionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

        // Reflect on the action method and its parameters to get attributes
        if (actionDescriptor != null)
        {
            var parameters = actionDescriptor.MethodInfo.GetParameters();
            foreach (var parameter in parameters)
            {
                var atr = parameter.GetCustomAttribute<FromBodyAttribute>();
                if (atr != null)
                {
                    var paramType = parameter.ParameterType;
                    foreach (var prop in paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var atrNonDef = prop.GetCustomAttribute<ValidateNotDefaultAttribute>();
                        if (atrNonDef != null)
                        {
                            var value = await context.Request.ReadFromJsonAsync(parameter.ParameterType);
                            var v = prop.GetValue(value);
                            if (IsDefault(v, prop.PropertyType))
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsJsonAsync(new BadRequest()
                                {
                                    fieldErrors = new[] { $"{prop.PropertyType.AssemblyQualifiedName}->{prop.Name} must be non default" },
                                });
                                return;
                            }
                        }
                    }
                }
            }
        }

        await next.Invoke(context);
    }

    private bool IsDefault(object? value, Type type)
    {
        if (type.IsClass) return value is null;
        return value.Equals(Activator.CreateInstance(type));
    }
}