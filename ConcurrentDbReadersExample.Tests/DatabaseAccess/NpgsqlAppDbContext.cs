using Microsoft.EntityFrameworkCore;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public sealed class NpgsqlAppDbContext : AppDbContext
{
    public NpgsqlAppDbContext(DbContextOptions<NpgsqlAppDbContext> options) : base(options) { }
}