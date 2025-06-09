using Microsoft.EntityFrameworkCore;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public sealed class OracleAppDbContext : AppDbContext
{
    public OracleAppDbContext(DbContextOptions<OracleAppDbContext> options) : base(options) { }
}