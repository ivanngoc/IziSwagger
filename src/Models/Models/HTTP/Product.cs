using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MockServer.Attributes;
using MockServer.Metas;
using MockServer.Metas.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace MockServer.Models;

public class Product
{
    /// <summary>
    ///  Код товара (GTIN).  Строковое значение.
    /// </summary>
    [SwaggerParameter(Required = true)]
    [DefaultValue(Constants.gtin)]
    public string gtin { get; set; }

    /// <summary>
    ///  Строка  (String) ($int32).
    ///  Количество КМ.  Положительное числовое значение больше нуля.
    /// </summary>
    [SwaggerParameter(Required = true), DefaultValue(4), ValidateNotDefault]
    public int quantity { get; set; }

    /// <summary>
    ///  Способ генерации серийных номеров.  Значение по умолчанию: OPERATOR
    /// </summary>
    [SwaggerParameter(Required = true)]
    [DefaultValue(ESerialNumberType.OPERATOR)]
    public ESerialNumberType serialNumberType { get; set; }

    /// <summary>
    /// Массив строк  (JSON Array of String*).
    /// Условно обязательное.
    /// Массив серийных номеров. Это поле указывается в случае, если значение «serialNumberType» = «SELF_MADE» 
    /// </summary>
    public string[]? serialNumbers { get; set; }

    /// <summary>
    ///  Идентификатор шаблона КМ.  Положительное числовое значение больше нуля.  Допустимые значения указаны в справочнике «Шаблоны КМ» (См. подпункт 2.3.1.1)
    /// </summary>
    [SwaggerParameter(Required = true)]
    public int templateId { get; set; }

    /// <summary>
    /// Тип кода маркировки. Допустимые значения указаны в справочнике «Тип кода маркировки» (См. подпункт 2.3.1.6)
    /// </summary>
    [SwaggerParameter(Required = true)]
    public ECisType cisType { get; set; }

    public Dictionary<string, string>? attributes { get; set; }
}