using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static MockServer.Metas.Constants;

namespace MockServer.Controllers;

/// <summary>
/// Количество КМ в отчёте об использовании не должно превышать 30 000 кодов.
/// Структура объекта «UtilisationReport», тело сообщения (HTTP Body).
/// </summary>
public class UtilisationReport
{
    /// <summary>
    /// Товарная группа. Допустимые значения справочника «Товарные группы» (См. подпункт 2.3.1.7)
    /// </summary>
    [Required, DefaultValue(EProductGroupe.construction)]
    public string productGroup { get; set; }

    /// <summary>
    ///  Массив строк (коды маркировки)
    /// </summary>
    [Required, DefaultValue(new string[] { CODE_2 })]
    public string[] sntins { get; set; }

    /// <summary>
    /// Словарь, состоящий из пар ключ/ значение, каждый ключ — это уникальный идентификатор в пределах словаря.
    /// </summary>
    /// <example>
    /// Пример:<br/>
    /// "attributes": {<br/>
    /// "attribute1": "value1",<br/>
    /// "attribute2": "value2"<br/>
    /// }<br/>
    /// Перечень допустимых атрибутов для конкретной товарной группы приведен в соответствующих подразделах данного раздела.
    /// </example>
    public Dictionary<string, string>? attributes { get; set; }
}