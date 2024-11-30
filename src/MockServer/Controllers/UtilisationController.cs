using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Attributes;
using MockServer.Databases;
using MockServer.Metas;
using MockServer.Metas.Enums;
using MockServer.Middlewares;
using MockServer.Models;
using MockServer.Tools;
using Swashbuckle.AspNetCore.Annotations;
using static MockServer.Metas.Constants;

namespace MockServer.Controllers;

[Route("[controller]")]
[ApiController]
public class UtilisationController : ControllerBase
{
    private readonly EquironDbContext context;

    public UtilisationController(EquironDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    ///  1.3.5.	Метод «Отправить отчёт об использовании (нанесении) КМ» 29<br/>
    ///  Этот метод используется для отправки отчёта об использовании КМ. Описание по получению маркера безопасности приведено в разделе 1.2.  По итогам отправки отчета об использовании КМ необходимо получить статус обработки отчета (используя Метод «Получить статус обработки отчёта»).
    /// </summary>
    /// <param name="omsId">
    /// Строка  (String)  (36)  (UUID).
    /// Уникальный идентификатор УОТ (эмитента).  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</param>
    /// <returns> При успешном выполнении запроса сервер возвращает HTTP код -200 и уникальный идентификатор отчёта об использовании КМ. Полученный идентификатор отчёта об использовании КМ используется для получения статуса обработки отчёта (см. раздел 1.3.6). Структуру ответа на запрос отправки отчёта об использовании отображает Таблица 32. Коды ошибок приведены в подразделе.</returns>
    /// <remarks>Чан Иван: Не думаю что нам нужно для мока делать очередь сообщений которая будет отдельно обрабатывать отчеты. пока прям тут все делаем</remarks>
    [HttpPost]
    [SwaggerHeader(ctype, apJson, "application/json;charset=UTF-8")]
    [SwaggerHeader("Accept", "application/json", "application/json")]
    [SwaggerHeader("Authorization", "Bearer {token}", JWTTokenHeaderValue, requiredValue: false)]
    [SwaggerHeader("X-Signature", "{подпись}", Signature, required: false),]
    [Produces("application/json", Type = typeof(Response135))]
    // http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886659
    public async Task<ActionResult> Action135(
        [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(omsIdEmittent), UUIDValidation]
        string omsId,
        [FromBody]
        UtilisationReport body)
    {
        if (body.sntins.Length == 0)
        {
            return base.BadRequest(new BadRequest()
            {
                fieldErrors = new[] { "sntins length is 0" },
                success = false,
            });
        }

        for (int i = 0; i < body.sntins.Length; i++)
        {
            string val = body.sntins[i];
            if (!ProductCodeGenerationTool.Validate(val))
            {
                return base.BadRequest(new BadRequest()
                {
                    fieldErrors = new[] { $"sntins[{i}] format error. Recived:{val}" },
                    success = false,
                });
            }
        }

        Guid guidOmsId = Guid.Parse(omsId);
        var member = await context.Members.FirstOrDefaultAsync(x => x.ModelMemberId == guidOmsId);

        if (member is null)
        {
            return base.BadRequest(new BadRequest()
            {
                globalErrors = new[] { $"omsId:{guidOmsId} Not exists" }
            });
        }

        var array = body.sntins;
        var pairs = (
            from code in array
            join model in context.Codes.Include(x => x.ModelMember).Include(x => x.BufferInfo)
                on code equals model.Value into temp
            from value in temp.DefaultIfEmpty()
            select new { code = code, model = value }).ToArray();

        foreach (var pair in pairs)
        {
            if (pair.model is null)
            {
                return base.BadRequest(new BadRequest()
                {
                    fieldErrors = new[]
                    {
                        $"code with value: {pair.code} is not founded in database",
                    }
                });
            }

            var code = pair.model;

            // код принадлежит другому юзеру. на дебаге можно сказать кому
            if (code.ModelMemberId != guidOmsId)
            {
                return base.BadRequest(new BadRequest()
                {
                    globalErrors = new[] { $"there is no code with value {code.Value} for user {guidOmsId}, but for {code.ModelMemberId}" }
                });
            }

            if (code.Status != ECodeStatus.Issued)
            {
                return base.BadRequest(new BadRequest()
                {
                    globalErrors = new[] { $"Code: {code.Value} has status: {code.Status}. Must be: {ECodeStatus.Issued}" }
                });
            }

            code.Status = ECodeStatus.Used;

            var buffer = code.BufferInfo;
            buffer.UnavailableCodes++;
            buffer.LeftInBuffer--;
            buffer.AvailableCodes--;
            buffer.TotalPassed = default; // ??
            buffer.PoolsExhausted = buffer.AvailableCodes == 0;
            buffer.Codes.Remove(code);
            if (buffer.AvailableCodes < 0) throw new ApplicationException("где-то что-то выполнилось неправильно. не сходится дебет с кредитом");
            if (buffer.PoolsExhausted)
            {
                buffer.BufferStatus = EBufferStatus.EXHAUSTED;
            }
        }

        var report = new ModelReportCodeUsage()
        {
            ModelReportCodeUsageId = Guid.NewGuid(),
            Codes = pairs.Select(x => x.model).ToArray(),
            Status = EReportStatus.SUCCESS,
            Member = member,
        };
        context.Reports.Add(report);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return Ok(new Response135()
        {
            omsId = omsIdEmittent,
            reportId = report.ModelReportCodeUsageId.ToString("D"),
        });
    }
}