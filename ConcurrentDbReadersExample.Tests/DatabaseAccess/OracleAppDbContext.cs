using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public sealed class OracleAppDbContext : AppDbContext
{
    public OracleAppDbContext(DbContextOptions<OracleAppDbContext> options) : base(options) { }

    public static OracleAppDbContext Create(string connectionString, ILogger logger) =>
        new (
            new DbContextOptionsBuilder<OracleAppDbContext>()
               .UseOracle(connectionString)
               .UseUpperSnakeCaseNamingConvention()
               .UseLoggerFactory(new SerilogLoggerFactory(logger))
               .Options
        );
}
