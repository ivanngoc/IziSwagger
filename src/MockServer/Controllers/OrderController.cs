using MockServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Attributes;
using MockServer.Databases;
using MockServer.Metas.Enums;
using MockServer.Middlewares;
using MockServer.Models;
using Stroyplatforma.HelpersForBCL;
using Swashbuckle.AspNetCore.Annotations;
using static MockServer.Controllers.HedersValidatation;
using static MockServer.Metas.Constants;
using static MockServer.Helpers.CopyHelper;

namespace MockServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly EquironDbContext context;
        private readonly ILogger<OrderController> logger;

        public OrderController(ILogger<OrderController> logger, EquironDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        /// <summary>
        /// 1.3.1 Создать заказ на эмиссию кодов маркировки
        /// </summary>
        /// <param name="omsId"> Уникальный идентификатор УОТ (эмитента). Строковое значение. Значение идентификатора в соответствии с ISO/IEC 9834-8. Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F] {4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[09a-fA-F]{12}
        ///<br/>
        /// CDF12109-10D3-11E6-8B6F-0050569977A1    
        /// </param>
        /// <returns>Метод возвращает уникальный идентификатор заказа и время планируемого выполнения заказа в миллисекундах (полученное время необходимо поделить на 1000, чтобы получить секунды и на 60, чтобы получить минуты). Значение «orderId» используется для получения КМ из заказа, когда заказ выполнен (См. пункт 1.3.3). Коды ошибок приведены в подразделе.</returns>
        // <example>http://localhost:5218/api/v3/order?omsId=CDF12109-10D3-11E6-8B6F-0050569977A1</example>
        // doc http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886647
        [HttpPost]
        [Produces("application/json", Type = typeof(Response131))]
        [SwaggerHeader(ctype, apJson, "application/json;charset=UTF-8")]
        [SwaggerHeader("Accept", "application/json", "application/json")]
        [SwaggerHeader("Authorization", "Bearer {token}", JWTTokenHeaderValue, requiredValue: false)]
        [SwaggerHeader("X-Signature", "{подпись}", Signature, required: false, requiredValue: false)]
        // [Authorize]
        public async Task<ActionResult> Action131(
            [FromQuery, SwaggerParameter(Required = true, Description = "some  descr"), SwaggerTryItOutDefaultValue(omsIdEmittent), UUIDValidation]
            string omsId,
            [FromBody]
            Order posted)
        {
            try
            {
                if (posted.products.Any(x => x.quantity == 0)) throw new Exception($"order.products[n].quantity is 0. Must be greater");
                var model = await CreateOrderAsync(omsId, posted).ConfigureAwait(false);
                return Ok(new Response131()
                {
                    omsId = omsIdEmittent,
                    // Метод возвращает уникальный идентификатор заказа
                    orderId = model.OrderId.ToString("D").ToLower(), // CreateOrder(),
                    // и время планируемого выполнения заказа в миллисекундах (полученное время необходимо поделить на 1000, чтобы получить секунды и на 60, чтобы получить минуты). 
                    expectedCompleteTimestamp = 5100, // (long)(DateTime.Now.AddSeconds(1) - DateTime.MinValue).TotalMilliseconds,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new BadRequest() { globalErrors = new[] { $"{e.Message}{Environment.NewLine}{e.StackTrace}" } });
            }
        }

        private async ValueTask<ModelOrder> CreateOrderAsync(string omsId, Order posted)
        {
            var omsIdAsguid = Guid.Parse(omsId);
            var model = await OrderToModelAsync(posted, omsIdAsguid).ConfigureAwait(false);
            var member = context.Members.Where(x => x.ModelMemberId == omsIdAsguid).First();
            model.Member = member;
            var e = await context.Orders.AddAsync(model).ConfigureAwait(false);
            context.ProcessingQueue.Add(new ModelProcessingQueueItem()
            {
                Id = default,
                Order = e.Entity,
                Member = member,
            });
            await context.SaveChangesAsync().ConfigureAwait(false);
            return e.Entity;
        }

        /// <summary>
        /// 1.3.2 Метод «Получить статус массива КМ из заказа». На первый запрос всегда статус bufferStatus=PENDING (невалидный), затем Active 
        /// </summary>
        /// <param name="omsId">Уникальный идентификатор УОТ (эмитента). Строковое значение. Значение идентификатора в соответствии с ISO/IEC 9834-8. Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[09a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
        /// <br/> cdf12109-10d3-11e6-8b6f-0050569977a1
        /// </param>
        /// <param name="orderId">Идентификатор заказа на эмиссию КМ. Строковое значение. Значение идентификатора в соответствии с ISO/IEC 9834-8. Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[09a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
        /// <br/> b024ae09-ef7c-449e-b461-05d8eb116c79
        /// </param>
        /// <param name="gtin">GTIN товара, по которому нужно получить статус заказа.
        /// <br/> 01334567894339
        /// </param>
        /// <returns></returns>
        // нельзя использовать ссылки в XML комментариях из-за безопасности?
        // http://localhost:5218/api/v3/order/status?orderId=b024ae09-ef7c-449e-b461-05d8eb116c79&omsId=cdf12109-10d3-11e6-8b6f-0050569977a1&gtin=01334567894339
        // Doc: http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886651
        [HttpGet("status")]
        [Produces("application/json", Type = typeof(BufferInfo))]
        [SwaggerHeader("Accept", "application/json", "application/json")]
        [SwaggerHeader("Authorization", "Bearer {token}", JWTTokenHeaderValue, requiredValue: false)]
        public async Task<ActionResult> Action132(
            [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(GUID_ORDER), UUIDValidation]
            string orderId,
            [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(omsIdEmittent), UUIDValidation]
            string omsId,
            [FromQuery, SwaggerTryItOutDefaultValue(gtin)]
            string gtin = default)
        {
            var prod = context.Products.FirstOrDefault(x => x.Gtin == gtin);
            if (prod is null)
            {
                return Ok(new BufferInfo()
                {
                    leftInBuffer = -1,
                    poolsExhausted = false,
                    totalCodes = -1,
                    unavailableCodes = -1,
                    gtin = gtin,
                    bufferStatus = EBufferStatus.REJECTED,
                    rejectionReason = $"Order declined: Контрольно-логическая проверка заказа не пройдена. 0106: Значение {gtin} в поле 'GTIN' в документе 'Заказ КМ' {orderId} не найдено в реестре GTIN.",
                    totalPassed = -1,
                    templateId = 50,
                });
            }

            Guid guid = Guid.Parse(orderId);
            var bi = context.Buffers.Include(x => x.ModelOrder).ThenInclude(y => y.Member).FirstOrDefault(x => x.ModelOrder.OrderId == guid);
            if (bi is null)
            {
                return NotFound(new BadRequest() { globalErrors = new[] { $"не найден буфер с orderId:{orderId}" } });
            }

            if (bi.ModelOrder.Member.ModelMemberId != Guid.Parse(omsId))
            {
                return BadRequest(new BadRequest()
                {
                    fieldErrors = new[] { $"omsId для ордера не совпадает переданным. для этого ордера omsid={bi.ModelOrder.Member.ModelMemberId}" },
                });
            }

            var result = ModelBufferInfoToHttp(bi);
            if (bi.META_RequestCount == 0)
            {
                bi.META_RequestCount++;
                await context.SaveChangesAsync();
                result.bufferStatus = EBufferStatus.PENDING;
            }
            else
            {
                bi.BufferStatus = EBufferStatus.ACTIVE;
                await context.SaveChangesAsync();
            }

            if (bi.BufferStatus != EBufferStatus.ACTIVE)
            {
                return Ok(new BufferInfo()
                {
                    leftInBuffer = -1,
                    poolsExhausted = false,
                    totalCodes = -1,
                    unavailableCodes = -1,
                    gtin = gtin,
                    bufferStatus = bi.BufferStatus,
                    totalPassed = -1,
                    templateId = 50,
                });
            }

            return Ok(result);

            return Ok(new BufferInfo()
            {
                leftInBuffer = 0,
                totalCodes = 20,
                poolsExhausted = false,
                unavailableCodes = 0,
                availableCodes = 20,
                gtin = "01334567894339",
                bufferStatus = EBufferStatus.ACTIVE,
                totalPassed = 0,
                expiredDate = 1596792681987,
                templateId = 50
            });
        }

        private BufferInfo ModelBufferInfoToHttp(ModelBufferInfo model)
        {
            return new BufferInfo()
            {
                leftInBuffer = model.LeftInBuffer,
                totalCodes = model.TotalCodes,
                poolsExhausted = model.PoolsExhausted,
                unavailableCodes = model.UnavailableCodes,
                availableCodes = model.AvailableCodes,
                gtin = model.Gtin,
                bufferStatus = model.BufferStatus,
                totalPassed = model.TotalPassed,
                expiredDate = model.ExpiredDate,
                templateId = model.TemplateId,
            };
        }


        /// <summary>
        ///  1.3.3.	Метод «Получить список заказов КМ» 22
        /// </summary>
        /// <param name="omsId"> Уникальный идентификатор УОТ (эмитента).  Строковое значение.  Значение идентификатора в соответствии с ISO/IEC 9834-8.  Шаблон: [0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
        /// </param>
        /// <returns> При успешном выполнении запроса сервер возвращает HTTP код 200 и данные статус бизнес заказов и Уникальный идентификатор УОТ (эмитента). Формат ответа на запрос Таблица 22. Коды ошибок приведены в подразделе.</returns>
        /// <exception cref="NotImplementedException"></exception>
        // http://localhost:63342/MockForEquiron/docs/%D0%9C%D0%BE%D0%B4%D1%83%D0%BB%D1%8C%20API%20%D0%93%D0%98%D0%A1_25.06.24_v0.2.html?_ijt=b05n58teluan8jh95db471f8cf&_ij_reload=RELOAD_ON_SAVE#__RefHeading___Toc169886653
        [HttpGet("list")]
        [SwaggerHeader("Accept", "application/json", "application/json")]
        [SwaggerHeader("Authorization", "Bearer {token}", JWTTokenHeaderValue, requiredValue: false)]
        [Produces("application/json", Type = typeof(Response133))]
        public ActionResult Action133(
            [FromQuery, SwaggerParameter(Required = true), SwaggerTryItOutDefaultValue(omsIdEmittent), UUIDValidation]
            string omsId)
        {
            Guid guid = Guid.Parse(omsId);
            var response = ToResponse133(guid);
            response.omsId = omsId;
            return Ok(response);

            return Ok(new Response133()
            {
                omsId = omsIdEmittent,
                orderInfos = new OrderSummaryInfo[]
                {
                    new()
                    {
                        orderId = orderIdToCreate,
                        orderStatus = EOrderStatus.READY,
                        createdTimestamp = 1550650989568,
                        productGroup = EProductGroupe.construction,
                        buffers = new[]
                        {
                            new BufferInfo()
                            {
                                leftInBuffer = 20,
                                totalCodes = 20,
                                unavailableCodes = 0,
                                gtin = gtin,
                                bufferStatus = EBufferStatus.ACTIVE,
                                templateId = 53,
                            }
                        },
                    }
                }
            });
        }

        private Response133 ToResponse133(Guid omsId)
        {
            var summaries = context.SummaryInfos
                .Include(x => x.Buffers)
                .Where(x => x.Member.ModelMemberId == omsId).ToArray();
            int count = summaries.Count();

            Response133 response133 = new Response133()
            {
                orderInfos = new OrderSummaryInfo[count],
            };

            for (int i = 0; i < count; i++)
            {
                var modelSummary = summaries[i];
                OrderSummaryInfo osi = new();
                CopyModelToTarget(modelSummary, osi);
                response133.orderInfos[i] = osi;
            }

            return response133;
        }

        [HttpPost("Test")]
        public ActionResult ActionOrderStatusTest([FromBody] OrderSummaryInfo body)
        {
            Console.WriteLine(body.orderStatus);
            body.orderStatus = EOrderStatus.DECLINED;
            return Ok(body);
        }

        private async ValueTask<ModelOrder> OrderToModelAsync(Order order, Guid omsId)
        {
            var productionOrderId = order.attributes is null ? default : Guid.Parse(order.attributes.productionOrderId);
            int count = order.products.Length;
            var buffers = new ModelBufferInfo[count];
            var products = new ModelProduct[count];
            var orderId = Guid.NewGuid();
            var model = new ModelOrder()
            {
                ModelOrderId = default,
                ProductGroup = order.productGroup,
                OrderId = orderId,
                ServiceProviderId = order.serviceProviderId,
                ModelOrderSummaryInfo = new ModelOrderSummaryInfo()
                {
                    Buffers = default,
                    Member = default,
                    ModelOrder = default,
                    OrderId = default,
                    DeclineReason = default,
                    ProductionOrderId = productionOrderId,
                    ModelOrderSummaryInfoId = default,
                    ModelMemberId = omsId,
                    CreatedTimestamp = DateTime.Now.ToUnixMilliseconds(),
                    OrderStatus = EOrderStatus.CREATED,
                    ProductGroup = order.productGroup,
                    PaymentType = default
                },
                ModelAttribute = new ModelAttribute()
                {
                    ContactPerson = order.attributes?.contactPerson,
                    CreateMethodType = order.attributes?.createMethodType,
                    ReleaseMethodType = order.attributes?.releaseMethodType,
                    ProductionOrderId = order.attributes is null ? default : Guid.Parse(order.attributes.productionOrderId),
                },
            };

            model.ModelOrderSummaryInfo.ProductionOrderId = model.OrderId;
            model.ModelOrderSummaryInfo.ModelOrder = model;
            model.ModelOrderSummaryInfo.OrderId = orderId;

            var existedProducts = context.Products.Where(x => order.products.Select(y => y.gtin).Contains(x.Gtin));
            for (int i = 0; i < order.products.Length; i++)
            {
                var reqProduct = order.products[i];
                var gtin = reqProduct.gtin;
                var product = existedProducts.FirstOrDefault(x => x.Gtin == reqProduct.gtin);
                if (product is null)
                {
                    throw new ArgumentNullException($"Product with gtin:{gtin} not founded");
                    product = new ModelProduct();
                    product.ModelProductId = default;
                    product.Gtin = reqProduct.gtin;
                    context.Add(product);
                }

                products[i] = product;
                product.CisType = reqProduct.cisType;
                // product.Attributes = reqProduct.attributes.ToArray();
                product.TemplateId = reqProduct.templateId;
                product.SerialNumberType = reqProduct.serialNumberType;
                var mbi = new ModelBufferInfo()
                {
                    ModelOrder = model,
                    BufferStatus = EBufferStatus.PENDING,
                    Product = product,
                };
                buffers[i] = mbi;
                CopyTargetToModel(reqProduct, mbi);

                mbi.PoolsExhausted = default;
                mbi.RejectionReason = default;
                mbi.TotalPassed = default;
                mbi.UnavailableCodes = default;
                mbi.ExpiredDate = DateTime.Now.AddDays(1).ToUnixMilliseconds();
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            model.Products = products;
            model.Buffers = buffers;
            model.ModelOrderSummaryInfo.Buffers = buffers;
            return model;
        }
    }
}