using MockServer;
using Microsoft.EntityFrameworkCore;
using MockServer.Models;

namespace MockServer.Databases;

public class EquironDbContext : DbContext
{
    public DbSet<ModelCode> Codes { get; set; }
    public DbSet<ModelOrder> Orders { get; set; }
    public DbSet<ModelMember> Members { get; set; }
    public DbSet<ModelProduct> Products { get; set; }
    public DbSet<ModelAttribute> Attributes { get; set; }
    public DbSet<ModelBufferInfo> Buffers { get; set; }
    public DbSet<ModelOrderSummaryInfo> SummaryInfos { get; set; }
    public DbSet<ModelProcessingQueueItem> ProcessingQueue { get; set; }

    public DbSet<ModelReportCodeUsage> Reports { get; set; }

    public EquironDbContext(DbContextOptions<EquironDbContext> opt) : base(opt)
    {
    }
}