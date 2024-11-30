namespace MockServer.Metas.Enums;

/// <summary>
/// статус кода маркировки
/// </summary>
[Flags]
public enum ECodeStatus
{
    None,
    /// <summary>
    ///  Выпущен
    /// </summary>
    Issued,
    /// <summary>
    /// Использован
    /// </summary>
    Used,
    /// <summary>
    /// Списанный код
    /// </summary>
    WriteOff,
    /// <summary>
    /// Просрочен
    /// </summary>
    Expired,
}