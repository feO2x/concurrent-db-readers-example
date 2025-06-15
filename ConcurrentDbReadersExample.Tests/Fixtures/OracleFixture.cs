using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.XUnit.Injectable;
using Serilog.Sinks.XUnit.Injectable.Extensions;
using Testcontainers.Oracle;
using Xunit;

namespace ConcurrentDbReadersExample.Tests.Fixtures;

public sealed class OracleFixture : IAsyncLifetime
{
    private readonly Logger _logger;
    
    public OracleFixture() =>
        _logger = new LoggerConfiguration()
           .WriteTo.InjectableTestOutput(TestOutputSink)
           .CreateLogger();

    private OracleContainer OracleContainer { get; } = new OracleBuilder()
       .WithImage("gvenzl/oracle-free:23.5-slim-faststart")
       .WithUsername("test")
       .WithPassword("testpassword")
       .WithPortBinding(1521, assignRandomHostPort: true)
       .Build();

    public InjectableTestOutputSink TestOutputSink { get; } = new ();

    public ILogger Logger => _logger;

    public async ValueTask InitializeAsync()
    {
        await OracleContainer.StartAsync();
        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await OracleContainer.StopAsync();
        await OracleContainer.DisposeAsync();
        await _logger.DisposeAsync();
    }

    public OracleAppDbContext CreateDbContext() =>
        OracleAppDbContext.Create(OracleContainer.GetConnectionString(), Logger);
}
