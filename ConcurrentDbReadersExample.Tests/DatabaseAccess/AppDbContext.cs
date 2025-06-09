using Microsoft.EntityFrameworkCore;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public abstract class AppDbContext : DbContext
{
    protected AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<OutboxItem> OutboxItems => Set<OutboxItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OutboxItem>(entity =>
        {
            entity.Property(e => e.Type)
                  .HasMaxLength(200);
            
            var jsonPayloadDbType = Database.IsNpgsql() ? "jsonb" : "JSON";
            entity.Property(e => e.JsonPayload)
                  .HasColumnType(jsonPayloadDbType)
                  .HasMaxLength(4000);

            entity.Property(e => e.ProcessedBy)
                  .HasMaxLength(200);

            var indexName = Database.IsNpgsql() ? "ix_outbox_items_unprocessed_items" : "IX_OUTBOX_ITEMS_UNPROCESSED_ITEMS";
            var columnName = Database.IsNpgsql() ? "processed_at_utc" : "PROCESSED_AT_UTC";
            entity.HasIndex(e => new { e.Id, e.ProcessedAtUtc })
                  .HasDatabaseName(indexName)
                  .HasFilter($"{columnName} IS NULL");
        });
    }
}