using Microsoft.EntityFrameworkCore.Design;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.EfMigrationsSupport;

// ReSharper disable once UnusedType.Global -- type is instatiated by the dotnet ef CLI tool
public sealed class NpgsqlAppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<NpgsqlAppDbContext>
{
    public NpgsqlAppDbContext CreateDbContext(string[] args) =>
        DesignTimeFactory.CreateDbContext("postgres", NpgsqlAppDbContext.Create, args);
}
