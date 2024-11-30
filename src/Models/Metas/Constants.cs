namespace MockServer.Metas;

public class Constants
{
    /// <summary>
    ///  Уникальный идентификатор УОТ (эмитента).
    /// </summary>
    public const string omsIdEmittent = "cdf12109-10d3-11e6-8b6f-0050569977a1";
    public const string omsIdMember = "DF6AD330-F66F-4627-99F8-D6C943CF4FE6";

    public const string apJson = "application/json";
    public const string ctype = "Content-Type";
    public const string orderIdToCreate = "b024ae09-ef7c-449e-b461-05d8eb116c79";
    public const string gtin = "01334567894339";

    public const string JWTTokenHeaderValue =
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

    public const string Signature = "";
    public const string Q_PARAM_NAME_OMSID = "omsId";
    
    
    
    // Debug
    public const string GUID_PROD_ORDER = "A5845307-6574-493D-A8E6-A635BB2E9E1C";
    public const string GUID_SERVICE_ID = "68F1ED9F-1793-4E0E-8B68-C246DFCCD5EC";
    public const string GUID_ORDER = "D60040C3-78DA-438A-A0B8-FB01851CE97F";
    public const int DEBUG_TEMPLATE_ID = 53;
    public const string REPORT_ID = "81C86F4F-E036-4E67-851F-C85CBC864700";
    public const string CODE_0_REPORTED = "16976CDF-2C9A-4A02-86D8-2F4CE7C47FC1";
    public const string CODE_1_REPORTED = "95C1D79A-562D-48D0-A02F-EC22DCE9E9FD";
    public const string CODE_2 = "51F1D1D6-EC7D-46A9-A6C3-A476C4421824";
    public const string CODE_3 = "9EAAE7D1-A7A8-42A9-9FF6-C7D084F78C81";
    public static string[] codeValues = new[]
    {
        CODE_0_REPORTED,
        CODE_1_REPORTED,
        CODE_2,
        CODE_3
    };
}