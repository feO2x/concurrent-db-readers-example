using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public sealed class NpgsqlAppDbContext : AppDbContext
{
    public NpgsqlAppDbContext(DbContextOptions<NpgsqlAppDbContext> options) : base(options) { }

    public static NpgsqlAppDbContext Create(string connectionString, ILogger logger) =>
        new (
            new DbContextOptionsBuilder<NpgsqlAppDbContext>()
               .UseNpgsql(connectionString)
               .UseSnakeCaseNamingConvention()
               .UseLoggerFactory(new SerilogLoggerFactory(logger))
               .Options
        );
}
