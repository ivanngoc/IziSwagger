using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Attributes;
using MockServer.Databases;
using MockServer.Metas;
using MockServer.Metas.Enums;
using MockServer.Middlewares;
using Swashbuckle.AspNetCore.Annotations;

namespace MockServer.Controllers;

[Route("[controller]")]
[ApiController]
public class CodesController : ControllerBase
{
    private readonly EquironDbContext context;

    public CodesController(EquironDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// 1.3.4.	Метод «Получить КМ из заказа КМ» 26
    /// </summary>
    /// <param name="omsId"> Уникальный идентификатор УОТ (эмитента).  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</param>
    /// <param name="orderId"> Идентификатор заказа на эмиссию КМ.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</param>
    /// <param name="quantity"> Количество запрашиваемых кодов (не должно превышать 150 000 кодов маркировки). Положительное число.</param>
    /// <param name="gtin"> Код товара (GTIN).  Строковое значение.</param>
    /// <returns> При успешном выполнении запроса сервер возвращает HTTP код -200 и массив КМ. Формат ответа на запрос получения КМ для заданного товара отображает Таблица 27. Коды ошибок приведены в подразделе</returns>
    [HttpGet]
    [SwaggerHeader("Accept", "application/json", "application/json")]
    [SwaggerHeader("Authorization", "Bearer {token}", Constants.JWTTokenHeaderValue, requiredValue: false)]
    [Produces("application/json", Type = typeof(Response134))]
    // http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886656
    //
    public ActionResult Action134(
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(Constants.omsIdEmittent), UUIDValidation]
        string omsId,
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(Constants.GUID_ORDER), UUIDValidation]
        string orderId,
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(15)]
        int quantity,
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(Constants.gtin)]
        string gtin
    )
    {
        Guid orderGuid = Guid.Parse(orderId);
        var order = context.Orders.FirstOrDefault(x => x.OrderId == orderGuid);

        if (order != default)
        {
            var buffer = context.Buffers.Include(x => x.Codes).First(x => x.Gtin == gtin && x.ModelOrder == order);

            if (false)
            {
                if (buffer.BufferStatus != EBufferStatus.ACTIVE)
                {
                    return base.BadRequest(new BadRequest()
                    {
                        globalErrors = new[] { $"buffer for order is not active. Current status: {buffer.BufferStatus}" },
                    });
                }
            }

            var result = new Response134()
            {
                omsId = omsId,
                codes = buffer.Codes.Select(x => x.Value).ToArray(),
                blockId = buffer.BlockId.ToString("D"),
                DEBUG_STATUSES = buffer.Codes.ToDictionary(x => x.Value, x => x.Status.ToString()),
            };
            return Ok(result);
        }
        else
        {
            return BadRequest(new BadRequest() { globalErrors = new[] { $"order with orderId: {orderId} not founded" } });
        }

        throw new NotImplementedException();
        return Ok(new Response134()
        {
            omsId = omsId,
            codes = new[]
            {
                "[)>0625PKRUA123OABC000001S50ABCD01"
            },
            blockId = "012cc7b0-c9e4-4511-8058-2de1f97a87b0",
        });
    }
}