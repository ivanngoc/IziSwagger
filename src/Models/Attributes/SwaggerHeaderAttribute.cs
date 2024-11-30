namespace MockServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerHeaderAttribute : Attribute
{
    public string Header { get; }

    /// <summary>
    /// одно из значений, разделеных ';'
    /// </summary>
    public string Value { get; }

    public string ExampleValue { get; }
    public bool Required { get; }
    public bool RequiredValue { get; }

    public SwaggerHeaderAttribute(string header, string value, string exampleValue = default, bool required = true, bool requiredValue = true) : base()
    {
        Header = header;
        Value = value;
        ExampleValue = exampleValue;
        Required = required;
        RequiredValue = requiredValue;
    }
}