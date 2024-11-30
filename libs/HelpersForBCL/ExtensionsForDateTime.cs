namespace Stroyplatforma.HelpersForBCL;

public static class ExtensionsForDateTime
{
    public static long ToUnixMilliseconds(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
    }
}