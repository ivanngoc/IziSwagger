using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Attributes;
using MockServer.Databases;
using MockServer.Middlewares;
using MockServer.Models;
using Swashbuckle.AspNetCore.Annotations;
using static MockServer.Controllers.HedersValidatation;
using static MockServer.Metas.Constants;

namespace MockServer.Controllers;

[Route("[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly EquironDbContext context;

    public ReportController(EquironDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// 1.3.6.	Метод «Получить статус обработки отчёта».
    /// Этот метод используется для получения статуса обработки отчёта (отчета об использовании (нанесении) КМ и отчета об агрегации КМ) используя следующие параметры: маркер безопасности и идентификатор УОТ, идентификатор отчёта. Описание по получению маркера безопасности приведено в разделе 1.2.
    /// </summary>
    /// <param name="omsId"> Строка  (String)  (36)  (UUID). Уникальный идентификатор УОТ (эмитента).  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</param>
    /// <param name="reportId"> Строка  (String)  (36)  (UUID). Уникальный идентификатор отчёта.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</param>
    /// <returns> При успешном выполнении запроса сервер возвращает HTTP код 200 и Уникальный идентификатор УОТ (эмитента) и статус обработки отчёта. Формат ответа на запрос на получение статуса обработки отчёта отображает Таблица 36. Коды ошибок приведены в подразделе .</returns>
    [HttpGet("info")]
    [SwaggerHeader("Accept", "application/json", "application/json")]
    [SwaggerHeader("Authorization", "Bearer {token}", JWTTokenHeaderValue, requiredValue: false)]
    [Produces("application/json", Type = typeof(Response136))]
    // http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886662
    public async Task<ActionResult> Action136(
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(omsIdEmittent), UUIDValidation]
        string omsId,
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(REPORT_ID), UUIDValidation]
        string reportId)
    {
        var guid = Guid.Parse(reportId);
        var report = await context.Reports
            .Include(x => x.Member)
            .Include(x => x.Codes)
            .FirstOrDefaultAsync(x => x.ModelReportCodeUsageId == guid);

        if (report is null)
        {
            return base.BadRequest(new BadRequest()
            {
                globalErrors = new[] { $"report with id: {reportId} is not founded" }
            });
        }

        if (report.Member.ModelMemberId != Guid.Parse(omsId))
        {
            return base.BadRequest(new BadRequest()
            {
                globalErrors = new[] { $"Report has different member owner (omsId) which is :{omsId}" }
            });
        }

        return Ok(new Response136()
        {
            omsId = omsIdEmittent,
            reportId = report.ModelReportCodeUsageId.ToString("D"),
            reportStatus = report.Status,
            DEBUG_CODES = report.Codes.Select(x => x.Value).ToArray()
        });
    }
}