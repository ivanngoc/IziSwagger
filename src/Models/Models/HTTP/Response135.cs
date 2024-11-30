using System.ComponentModel.DataAnnotations;

namespace MockServer.Controllers;

/// <summary>
///  Таблица 32 – Формат ответа на запрос отправки отчёта о нанесении КМ
/// </summary>
public class Response135
{
    /// <summary>
    ///  Строка  (String)  (36)  (UUID).
    ///  Уникальный идентификатор УОТ (эмитента).  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string omsId { get; set; }

    /// <summary>
    ///  Строка  (String)  (36)  (UUID).
    ///  Уникальный идентификатор отчёта об нанесении КМ.  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12} 
    /// </summary>
    [Required]
    public string reportId { get; set; }
}