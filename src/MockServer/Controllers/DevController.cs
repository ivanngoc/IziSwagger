using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Attributes;
using MockServer.Databases;
using MockServer.Metas.Enums;
using Swashbuckle.AspNetCore.Annotations;
using static MockServer.Metas.Constants;

namespace MockServer.Controllers;

[Route("[controller]")]
[ApiController]
public class DevController : ControllerBase
{
    private readonly EquironDbContext context;

    public DevController(EquironDbContext context)
    {
        this.context = context;
    }


    [HttpPost("SetReady")]
    public async Task<ActionResult> SetReady(
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(orderIdToCreate), UUIDValidation]
        string orderId)
    {
        Guid guid = Guid.Parse(orderId);
        var order = context.Orders.Include(x => x.ModelOrderSummaryInfo).FirstOrDefault(x => x.OrderId == guid);
        var before = order.ModelOrderSummaryInfo.OrderStatus;
        order.ModelOrderSummaryInfo.OrderStatus = EOrderStatus.READY;
        await context.SaveChangesAsync();
        return Ok($"Stauts before:{before}; status after:{order.ModelOrderSummaryInfo.OrderStatus}");
    }


    [HttpPost("ApproveGisMt")]
    public async Task<ActionResult> ApproveGisMT(
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(orderIdToCreate), UUIDValidation]
        string orderId)
    {
        Guid guid = Guid.Parse(orderId);
        var order = context.Orders.Include(x => x.ModelOrderSummaryInfo).FirstOrDefault(x => x.OrderId == guid);
        var before = order.ModelOrderSummaryInfo.OrderStatus;
        order.ModelOrderSummaryInfo.OrderStatus = EOrderStatus.APPROVED;
        await context.SaveChangesAsync();
        return Ok($"Stauts before:{before}; status after:{order.ModelOrderSummaryInfo.OrderStatus}");
    }
}