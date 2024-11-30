using System.ComponentModel.DataAnnotations;

namespace MockServer.Controllers;

/// <summary>
/// ответ на запрос получения статуса заказа
/// </summary>
public class Response133
{
    /// <summary>
    /// Строка  (36)  (UUID).
    /// Уникальный идентификатор УОТ (эмитента).  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string omsId { get; set; }

    /// <summary>
    /// Массив заказов и их статус
    /// </summary>
    public OrderSummaryInfo[]? orderInfos { get; set; }
}