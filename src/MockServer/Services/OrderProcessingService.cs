using Microsoft.EntityFrameworkCore;
using MockServer.Databases;
using MockServer.Metas.Enums;
using MockServer.Models;
using MockServer.Tools;

namespace MockServer.Services;

public class OrderProcessingService : BackgroundService
{
    private readonly ILogger<OrderProcessingService> logger;
    private readonly IServiceProvider serviceProvider;

    public OrderProcessingService(ILogger<OrderProcessingService> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EquironDbContext>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var item = context.ProcessingQueue
                            .Include(x => x.Member)
                            .Include(x => x.Order)
                            .ThenInclude(y => y.Buffers)
                            .ThenInclude(z => z.Product)
                            .AsSplitQuery()
                            .FirstOrDefault();

                        if (item != null)
                        {
                            var member = item.Member ?? throw new NullReferenceException("Member is null");
                            var summary = context.SummaryInfos.Include(x => x.ModelOrder).First(x => x.ModelOrder == item.Order);
                            summary.OrderStatus = EOrderStatus.READY;
                            var order = summary.ModelOrder;

                            foreach (var buffer in item.Order.Buffers)
                            {
                                buffer.BufferStatus = EBufferStatus.PENDING;
                                int countToCreate = buffer.TotalCodes;
                                var codes = new ModelCode[countToCreate];
                                var product = buffer.Product;
                                buffer.Codes = codes;
                                buffer.BlockId = Guid.NewGuid();
                                for (int i = 0; i < countToCreate; i++)
                                {
                                    var code = new ModelCode()
                                    {
                                        Value = ProductCodeGenerationTool.GenerateNew(),
                                        Status = ECodeStatus.Issued,
                                        ModelMember = member,
                                        ModelProduct = product,
                                        BufferInfo = buffer,
                                        ModelOrder = order
                                    };
                                    codes[i] = code;
                                }
                            }

                            item.Order = default;
                            context.Remove(item);

                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                }
            }
        });
    }
}