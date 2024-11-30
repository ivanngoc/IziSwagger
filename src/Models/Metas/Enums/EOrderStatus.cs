using MockServer.Attributes;

namespace MockServer.Metas.Enums;

/// <summary>
/// ГИС МТ - Государственная информационная система мониторинга оборота товаров 
/// </summary>
// http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886683
public enum EOrderStatus
{
    None = 0,

    [Title("Заказ создан")]
    CREATED = 1,

    [Title("Заказ ожидает подтверждения ГИС МТ")]
    PENDING = 2,

    [Title("Заказ не подтверждён в ГИС МТ")]
    DECLINED = 3,

    [Title("Заказ подтверждён в ГИС МТ")]
    APPROVED = 4,

    [Title("Заказ готов")]
    READY = 5,

    [Title("Заказ закрыт")]
    CLOSED = 6,
}