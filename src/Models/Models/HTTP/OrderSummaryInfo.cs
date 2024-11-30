using System.ComponentModel.DataAnnotations;
using MockServer.Metas.Enums;

namespace MockServer.Controllers;

/// <summary>
/// OrderSummaryInfo
/// </summary>
public class OrderSummaryInfo
{
    /// <summary>
    /// Строка  (36)  (UUID).
    /// Идентификатор заказа на эмиссию КМ.  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    [Required]
    public string orderId { get; set; }

    /// <summary>
    /// Статус заказа. Допустимые значения указаны в справочнике «Статус заказа» (См. подпункт 2.3.1.5)
    /// </summary>
    [Required]
    public EOrderStatus orderStatus { get; set; }

    /// <summary>
    /// Информация о статусе буфера
    /// </summary>
    [Required]
    public BufferInfo[] buffers { get; set; }

    /// <summary>
    ///  Время создания заказа
    /// </summary>
    [Required]
    public long createdTimestamp { get; set; }

    /// <summary>
    /// Причина отклонения заказа
    /// </summary>
    public string? declineReason { get; set; }

    /// <summary>
    ///  Идентификатор производственного заказа
    /// </summary>
    public string? productionOrderId { get; set; }

    /// <summary>
    /// Товарная группа. Допустимые значения указаны в справочнике «Товарные группы» (см. раздел 2.3.1.7)
    /// </summary>
    [Required]
    public EProductGroupe productGroup { get; set; }

    /// <summary>
    ///  Тип оплаты
    /// </summary>
    public int? paymentType { get; set; }
}