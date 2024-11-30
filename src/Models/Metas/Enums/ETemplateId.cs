using MockServer.Attributes;

namespace MockServer.Metas.Enums;

/// <summary>
/// Возможные значения справочника «Шаблон КМ» templateId
/// </summary>
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886679
public enum ETemplateId
{
    None,

    [Title("Строительные материалы")]
    BuildingMaterials53 = 53,

    [Title("Строительные материалы")]
    BuildingMaterials54 = 54,
}

/// <summary>
///  «Статус буфера КМ» (См. Раздел 2.3.1.3)
/// </summary>
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886681
public enum EBufferStatus
{
    None,
    PENDING = 1,
    ACTIVE = 2,
    EXHAUSTED = 3,
    REJECTED = 4,
    CLOSED = 5,
}