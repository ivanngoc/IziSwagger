using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MockServer.Metas.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace MockServer.Controllers;

/// <summary>
/// объект «BufferInfo»
/// Таблица 18 – Формат ответа на запрос, объект «BufferInfo»
/// </summary>
public class BufferInfo
{
    /// <summary>
    /// Общее количество доступных КМ для товара в буфере и пулах регистратора
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int availableCodes { get; set; }

    /// <summary>
    /// Статус буфера. Допустимые значения указаны в справочнике «Статус буфера КМ» (См. Раздел 2.3.1.3)
    /// Значения: REJECTED (буфер существует, для отклонённого заказа); ACTIVE (буфер существует); PENDING (для отклонённого заказа, буфер неактивен)
    /// </summary>
    [SwaggerParameter(Required = true)]
    public EBufferStatus bufferStatus { get; set; } = EBufferStatus.None;

    /// <summary>
    /// GTIN – по которому был сделал запрос
    /// </summary>
    [SwaggerParameter(Required = true)]
    public string gtin { get; set; } = string.Empty;

    /// <summary>
    /// Количество неиспользованных КМ. (локальный буфер)
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int leftInBuffer { get; set; }

    /// <summary>
    /// Пулы КМ в регистраторах исчерпаны
    /// </summary>
    [SwaggerParameter(Required = true)]
    public bool poolsExhausted { get; set; }

    /// <summary>
    /// Причина отклонения буфера со стороны системы
    /// </summary>
    public string? rejectionReason { get; set; }

    /// <summary>
    /// Заказанное количество КМ в заказе
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int totalCodes { get; set; }

    /// <summary>
    /// Суммарное кол-во КМ полученных из буфера
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int totalPassed { get; set; }

    /// <summary>
    /// Количество недоступных кодов
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int unavailableCodes { get; set; }

    /// <summary>
    /// Дата истечения срока годности КМ Формат: UnixTime (в миллисекундах)
    /// </summary>
    /// <remarks>
    /// может прийти вот так: "expiredDate": "1596792681987",
    /// </remarks>
    public long? expiredDate { get; set; }

    /// <summary>
    /// Тип кода маркировки. Допустимые значения указаны в справочнике «Тип кода маркировки» (см. раздел 2.3.1.6)
    /// </summary>
    public int? cisType { get; set; }

    /// <summary>
    /// Идентификатор шаблона КМ. Допустимые значения указаны в справочнике «Шаблоны КМ» (см. раздел 2.3.1.1)
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int templateId { get; set; }
}