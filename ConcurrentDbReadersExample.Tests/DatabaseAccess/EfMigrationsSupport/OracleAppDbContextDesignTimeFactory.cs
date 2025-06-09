using Microsoft.EntityFrameworkCore.Design;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.EfMigrationsSupport;

// ReSharper disable once UnusedType.Global -- type is instatiated by the dotnet ef CLI tool
public sealed class OracleAppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<OracleAppDbContext>
{
    public OracleAppDbContext CreateDbContext(string[] args) =>
        DesignTimeFactory.CreateDbContext("oracle", OracleAppDbContext.Create, args);
}
