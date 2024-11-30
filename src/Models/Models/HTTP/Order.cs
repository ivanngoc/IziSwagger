using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MockServer.Attributes;
using MockServer.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace MockServer.Models;

/// <summary>
/// Order. запроса создания и отправки заказа на эмиссию КМ, объект «Order», тело сообщения (HTTP Body) 
/// </summary>
//  Таблица 11 – Описание формата JSON запроса создания и отправки заказа на эмиссию КМ, объект «Order», тело сообщения (HTTP Body) 
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886648
public class Order
{
    /// <summary>
    ///  Товарная группа. Допустимые значения справочника «Товарные группы» (См. подпункт 2.3.1.7)
    /// </summary>
    [Required]
    public EProductGroupe productGroup { get; set; }

    /// <summary>
    ///  Массив товаров заказа КМ
    /// </summary>
    [Required]
    public Product[] products { get; set; }

    /// <summary>
    ///  Строка  (36)  (UUID).
    ///  Идентификатор сервис-провайдера.  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
    /// </summary>
    public string? serviceProviderId { get; set; }

    /// <summary>
    /// (Условно обязательное)
    /// Словарь (Json Object)
    /// Словарь, состоящий из пар ключ/ значение, каждый ключ — это уникальный идентификатор в пределах словаря.
    /// Перечень допустимых атрибутов для конкретной товарной группы приведен в соответствующих подразделах данного раздела. 
    /// </summary>
    /// <example>
    /// Пример:
    /// "attributes": { 
    /// "attribute1": "value1", 
    /// "attribute2": "value2" 
    /// }
    /// </example> 
    public Attribute? attributes { get; set; }

    /// <summary>
    /// attributes объекта «Order» 
    /// </summary>
    [NotMapped]
    public class Attribute
    {
        /// <summary>
        ///  Строка (1-128). Контактное лицо
        /// </summary>
        public string contactPerson { get; set; }

        /// <summary>
        ///  Способ выпуска товаров в оборот. Значение по умолчанию: PRODUCTION
        /// </summary>
        [Required, DefaultValue("PRODUCTION")]
        public string releaseMethodType { get; set; }

        /// <summary>
        ///  Способ изготовления СИ. Значение по умолчанию: SELF_MADE
        /// </summary>
        [Required, SwaggerParameter(Required = true), DefaultValue("SELF_MADE")]
        public string createMethodType { get; set; }

        /// <summary>
        /// Строка (1-256).
        /// Идентификатор производственного заказа.  Не допускается указание в качестве значения пустой строки или строки, состоящей только из пробелов. Также не допускается указание непечатаемых специальных символов.
        /// </summary>
        [DefaultValue("9E72F6EA-DC94-4AF0-89B5-0C3749F07671")]
        public string productionOrderId { get; set; }
    }
}