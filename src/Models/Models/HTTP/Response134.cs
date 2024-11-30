using System.ComponentModel.DataAnnotations;

namespace MockServer.Controllers;

/// <summary>
/// Формат ответа на запрос получения КМ для заданного товара
/// </summary>
public class Response134
{
    /// <summary>
    ///  Строка  (36) (UUID).
    ///  Уникальный идентификатор УОТ (эмитента).  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string omsId { get; set; }

    /// <summary>
    ///  Массив КМ.
    /// </summary>
    [Required]
    public string[] codes { get; set; }

    /// <summary>
    /// Строка  (36) (UUID).
    /// Идентификатор пакета КМ.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string blockId { get; set; }

    public Dictionary<string, string>? DEBUG_STATUSES { get; set; }
}