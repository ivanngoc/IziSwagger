namespace MockServer.Metas.Enums;

/// <summary>
/// 
/// </summary>
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886682
public enum EReportStatus
{
    None,

    /// <summary>
    /// Отчет получен Системой (Черновик)
    /// </summary>
    DRAFT,

    /// <summary>
    ///  Отчет находится в ожидании
    /// </summary>
    PENDING,

    /// <summary>
    ///  Отчет готов к отправке в РЭ
    /// </summary>
    READY_TO_SEND,

    /// <summary>
    ///  Отчет отклонен
    /// </summary>
    REJECTED,

    /// <summary>
    ///  Отчет отправлен
    /// </summary>
    SENT,

    /// <summary>
    ///  Выполняется проверка метаданных отчёта (применимо к товарной группе «Лекарственные препараты для медицинского применения»)
    /// </summary>
    CHECK,

    /// <summary>
    ///  Отчёт обработан успешно
    /// </summary>
    SUCCESS,

    /// <summary>
    ///  Отчёт обработан с ошибкой
    /// </summary>
    FAILED,

    /// <summary>
    ///  Отчёт обработан частично
    /// </summary>
    PARTIALLY
}