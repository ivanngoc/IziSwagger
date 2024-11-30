using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyContext = MockServer.Databases.EquironDbContext;

namespace MockServer.Databases;

[ExcludeFromCodeCoverage]
internal class ContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyContext>
{
    public MyContext CreateDbContext(string[] args)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<MyContext>();
        dbContextBuilder.UseNpgsql("server=localhost;port=5432;uid=postgres;pwd=postgres;database=MockEquiron");
        return new MyContext(dbContextBuilder.Options);
    }
}