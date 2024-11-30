namespace MockServer.Tools;

public class ProductCodeGenerationTool
{
    public static string GenerateNew()
    {   
        /// TODO: Чан Иван 20240712 когда дадут формат кода маркировки переделать на требуемый. пока что затычка  
        return Guid.NewGuid().ToString("D");
    }

    public static bool Validate(string code)
    {
        return Guid.TryParse(code, out var guid);
    }
}