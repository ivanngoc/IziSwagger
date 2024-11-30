using Microsoft.EntityFrameworkCore;
using MockServer.Controllers;
using MockServer.Databases;
using MockServer.Metas;
using MockServer.Metas.Enums;
using MockServer.Models;
using Stroyplatforma.HelpersForBCL;
using static MockServer.Metas.Constants;

namespace MockServer;

public class InitSeeder
{
    private readonly DbContextOptions<EquironDbContext> opt;
    public static DateTime DateTimeCreate = new DateTime(2024, 07, 15, 11, 43, 55);

    public InitSeeder(DbContextOptions<EquironDbContext> opt)
    {
        this.opt = opt;
    }

    public async Task RecreateAsync()
    {
        using EquironDbContext db = new EquironDbContext(opt);
        await RecreateStatic(db);
    }

    public static async Task RecreateStatic(EquironDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Members.Add(new ModelMember()
        {
            ModelMemberId = Guid.Parse(omsIdEmittent),
        });
        /*
        "gtin": "04600682409427",
        "quantity": 2,
        "serialNumberType": "OPERATOR",
        "templateId": 53,
        "cisType": "UNIT"

         */
        db.Products.Add(new ModelProduct()
        {
            Gtin = gtin,
            CisType = ECisType.UNIT,
            // Attributes = new Dictionary<string, string>(),
            TemplateId = 53,
            SerialNumberType = ESerialNumberType.OPERATOR,
            ModelProductId = default,
        });
        await db.SaveChangesAsync().ConfigureAwait(false);
        await FillWithDummyOrders(db).ConfigureAwait(false);
    }

    public static async Task FillWithDummyOrders(EquironDbContext db)
    {
        var orderId = Guid.Parse(GUID_ORDER);
        var memberId = Guid.Parse(omsIdEmittent);
        var product = db.Products.First();
        var member = await db.Members.FindAsync(memberId) ?? throw new NullReferenceException(memberId.ToString("D"));
        var productionOrderId = Guid.Parse(GUID_PROD_ORDER);
        var msi = new ModelOrderSummaryInfo()
        {
            Member = member,
            ModelMemberId = member.ModelMemberId,
            ProductionOrderId = productionOrderId,
            OrderStatus = EOrderStatus.READY,
            OrderId = orderId,
            PaymentType = null,
            CreatedTimestamp = DateTimeCreate.ToUnixMilliseconds(),
            ModelOrderSummaryInfoId = default,
            ModelOrderId = default,
            ModelOrder = default,
            ProductGroup = EProductGroupe.construction,
            Buffers = default,
            DeclineReason = default
        };

        var buffer = new ModelBufferInfo()
        {
            ModelBufferInfoId = default,
            Gtin = product.Gtin,
            Product = product,
            BlockId = Guid.NewGuid(),
            ModelOrderSummaryInfo = msi,
            CisType = ECisType.UNIT,
            TemplateId = DEBUG_TEMPLATE_ID,
            BufferStatus = EBufferStatus.ACTIVE,
            ExpiredDate = DateTime.Now.AddDays(100).ToUnixMilliseconds(),
            PoolsExhausted = false,
            RejectionReason = default,
            TotalPassed = 0,
            UnavailableCodes = 2,
            TotalCodes = codeValues.Length,
            AvailableCodes = codeValues.Length - 2,
            LeftInBuffer = codeValues.Length - 2,

            SerialNumbers = default,
            ModelOrder = default,
            Codes = default,
        };
        var codes = new List<ModelCode>();

        for (int i = 0; i < codeValues.Length; i++)
        {
            var code = new ModelCode()
            {
                Id = default,
                ModelProduct = product,
                BufferInfo = buffer,
                ModelMember = member,
                Value = codeValues[i],
                Status = i > 1 ? ECodeStatus.Issued : ECodeStatus.Used,
            };
            codes.Add(code);
        }

        buffer.Codes = codes;

        var order = new ModelOrder()
        {
            OrderId = orderId,
            Buffers = new List<ModelBufferInfo>()
            {
                buffer,
            },
            Products = new List<ModelProduct>() { product },
            Member = member,
            MemberId = default,
            ModelOrderId = default,
            ProductGroup = EProductGroupe.construction,
            ModelOrderSummaryInfo = msi,
            ServiceProviderId = GUID_SERVICE_ID,
            ModelAttribute = new ModelAttribute()
            {
                ContactPerson = "Иванов П.А.",
                CreateMethodType = "SELF_MADE",
                ReleaseMethodType = "PRODUCTION",
                ProductionOrderId = productionOrderId,
            },
        };

        foreach (var code in codes)
        {
            code.ModelOrder = order;
        }

        buffer.ModelOrder = order;
        // msi.ModelOrder

        var report = new ModelReportCodeUsage()
        {
            ModelReportCodeUsageId = Guid.Parse(REPORT_ID),
            Codes = codes.Take(2).ToArray(),
            Member = member,
            Status = EReportStatus.SUCCESS,
        };

        db.Orders.Add(order);
        db.Reports.Add(report);
        await db.SaveChangesAsync();
    }
}