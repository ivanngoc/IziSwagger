using System.ComponentModel.DataAnnotations;

namespace MockServer.Controllers;

public class Response131
{
    /// <summary>
    /// Уникальный идентификатор СУЗ. Строковое значение. Значение идентификатора в соответствии с ISO/IEC 9834-8.
    /// Шаблон: [0-9a-fA-F]{8}-[09a-fA-F]{4}-[0-9a-fA-F]{4}[0-9a-fA-F]{4}-[0-9a-fA-F] {12}
    /// </summary>
    [Required]
    public string omsId { get; set; }

    /// <summary>
    /// Уникальный идентификатор заказа на эмиссию КМ. Строковое значение. Значение идентификатора в соответствии с ISO/IEC 9834-8.
    /// Шаблон: [0-9a-fA-F]{8}-[09a-fA-F]{4}-[0-9a-fA-F]{4}[0-9a-fA-F]{4}-[0-9a-fA-F] {12}
    /// </summary>
    [Required]
    public string orderId { get; set; }

    /// <summary>
    /// Время планируемого выполнения заказа в миллисекундах
    /// </summary>
    [Required]
    public long expectedCompleteTimestamp { get; set; }
}