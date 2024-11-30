using System.Reflection;
using MockServer.Attributes;
using System.Text.Json;
using Models.Contracts;

namespace MockServer.Middlewares;

public class ValidateHeadersMiddleware : IHeadersValidator
{
    private readonly RequestDelegate next;

    public ValidateHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint() ?? throw new NullReferenceException();

        var headerAtrs = endpoint.Metadata.Select(x => x as SwaggerHeaderAttribute).Where(x => x != null);

        foreach (var atr in headerAtrs)
        {
            if (atr.Required)
            {
                Console.WriteLine($"{atr.Header};{atr.Value}");
                if (context.Request.Headers.TryGetValue(atr.Header, out var headerValues))
                {
                    if (atr.RequiredValue)
                    {
                        foreach (var val in headerValues)
                        {
                            foreach (var itemVal in val.Split(';'))
                            {
                                if (itemVal.Trim().Equals(atr.Value)) goto NEXT_ATR;
                                else
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    BadRequest badRequest = new BadRequest();
                                    var v = $"Header '{atr.Header}' missing value: {atr.Value}";
                                    badRequest.globalErrors = new[] { v };
                                    return context.Response.WriteAsync(JsonSerializer.Serialize(badRequest));
                                }
                            }
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    BadRequest badRequest = new BadRequest();
                    var v = $"Missing header: '{atr.Header}'";
                    badRequest.globalErrors = new[] { v };
                    return context.Response.WriteAsync(JsonSerializer.Serialize(badRequest));
                }
            }

            NEXT_ATR:
            continue;
        }

        return next.Invoke(context);
    }
}