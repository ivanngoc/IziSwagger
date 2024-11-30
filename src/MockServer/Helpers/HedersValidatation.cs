namespace MockServer.Controllers;

public static class HedersValidatation
{
    public static bool HeaderHas(HttpRequest request, string header, string value)
    {
        if (request.Headers.ContainsKey(header))
        {
            if (request.Headers[header].First()!.Split(';').Select(x=>x.Trim()).Any(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }
        }
        return false;
    }
}