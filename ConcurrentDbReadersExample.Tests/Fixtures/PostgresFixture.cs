using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.XUnit.Injectable;
using Serilog.Sinks.XUnit.Injectable.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace ConcurrentDbReadersExample.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by the xunit runner
public sealed class PostgresFixture : IAsyncLifetime
{
    public PostgresFixture() =>
        Logger = new LoggerConfiguration()
           .WriteTo.InjectableTestOutput(TestOutputSink)
           .CreateLogger();

    private PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder()
       .WithImage("postgres:17.5")
       .WithDatabase("concurrent_db_readers")
       .WithUsername("postgres")
       .WithPassword("postgres")
       .WithPortBinding(5432, assignRandomHostPort: true)
       .Build();

    public InjectableTestOutputSink TestOutputSink { get; } = new ();
    public ILogger Logger { get; }

    public async ValueTask InitializeAsync()
    {
        await PostgresContainer.StartAsync();
        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }

    public NpgsqlAppDbContext CreateDbContext() =>
        NpgsqlAppDbContext.Create(PostgresContainer.GetConnectionString(), Logger);
}
