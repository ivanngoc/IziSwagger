using MockServer.Controllers;
using MockServer.Models;
using NuGet.ContentModel;

namespace MockServer.Helpers;

public class CopyHelper
{
    public static void CopyModelToTarget(ModelBufferInfo mbi, BufferInfo bi)
    {
        bi.leftInBuffer = mbi.LeftInBuffer;
        bi.totalCodes = mbi.TotalCodes;
        bi.poolsExhausted = mbi.PoolsExhausted;
        bi.availableCodes = mbi.AvailableCodes;
        bi.bufferStatus = mbi.BufferStatus;
        bi.gtin = mbi.Gtin;
        bi.rejectionReason = mbi.RejectionReason;
        bi.totalPassed = mbi.TotalPassed;
        bi.unavailableCodes = mbi.UnavailableCodes;
        bi.expiredDate = mbi.ExpiredDate;
        bi.cisType = (int)(mbi.CisType ?? 0);
        bi.templateId = mbi.TemplateId;
    }

    public static void CopyModelToTarget(ModelOrderSummaryInfo mosi, OrderSummaryInfo osi)
    {
        var modelBuffers = mosi.Buffers.ToArray();
        var buffers = new BufferInfo[mosi.Buffers.Count];

        for (int j = 0; j < buffers.Length; j++)
        {
            var mbi = modelBuffers[j];
            var bi = new BufferInfo();
            CopyModelToTarget(mbi, bi);
            buffers[j] = bi;
        }

        osi.buffers = buffers;
        osi.orderId = mosi.OrderId.ToString("D");
        osi.orderStatus = mosi.OrderStatus;
        osi.createdTimestamp = mosi.CreatedTimestamp;
        osi.declineReason = mosi.DeclineReason;
        osi.productionOrderId = mosi.ProductionOrderId.ToString("D");
        osi.productGroup = mosi.ProductGroup;
        osi.paymentType = mosi.PaymentType;
    }

    public static void CopyTargetToModel(Product product, ModelBufferInfo mbi)
    {
        // skip
        // mbi.BufferStatus = product.;

        mbi.Gtin = product.gtin;
        mbi.TotalCodes = product.quantity;
        mbi.AvailableCodes = product.quantity;
        mbi.LeftInBuffer = product.quantity;
        mbi.PoolsExhausted = false;
        
        mbi.RejectionReason = default;
        mbi.TotalPassed = default;
        mbi.UnavailableCodes = default;
        mbi.ExpiredDate = default;

        mbi.Gtin = product.gtin;
        mbi.CisType = product.cisType;
        mbi.TemplateId = product.templateId;
        mbi.SerialNumbers = product.serialNumbers;
    }
}