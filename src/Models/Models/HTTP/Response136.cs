using System.ComponentModel.DataAnnotations;
using MockServer.Metas.Enums;

namespace MockServer.Controllers;

/// <summary>
///  Таблица 36 – Формат ответа на запрос получения статуса обработки отчёта
/// </summary>
public class Response136
{
    /// <summary>
    ///  Уникальный идентификатор УОТ (эмитента).  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string omsId { get; set; }

    /// <summary>
    ///  Уникальный идентификатор отчёта.  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string reportId { get; set; }

    /// <summary>
    /// Статус обработки отчёта. Допустимые значения указаны в справочнике «Статус обработки отчета» (См. подпункт 2.3.1.4)
    /// </summary>
    [Required]
    public EReportStatus reportStatus { get; set; }

    /// <summary>
    ///  Причина отклонения отчета (обнаруженная ошибка).  Заполняется только при reportStatus "REJECTED")
    /// </summary>
    public string? errorReason { get; set; }

    public string[]? DEBUG_CODES { get; set; }
}