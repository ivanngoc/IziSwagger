namespace MockServer.Middlewares;

/// <summary>
/// 
/// </summary>
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886692
public class BadRequest
{
    public string[] fieldErrors { get; set; }
    public string[] globalErrors { get; set; }
    public bool success { get; set; }
}