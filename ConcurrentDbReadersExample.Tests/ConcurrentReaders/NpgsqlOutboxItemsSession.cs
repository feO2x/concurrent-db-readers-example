using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.DatabaseAccess;
using Light.DatabaseAccess.EntityFrameworkCore;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;

namespace ConcurrentDbReadersExample.Tests.ConcurrentReaders;

public sealed class NpgsqlOutboxItemsSession : EfSession<NpgsqlAppDbContext>.WithTransaction, IOutboxItemSession
{
    public NpgsqlOutboxItemsSession(NpgsqlAppDbContext dbContext) : base(dbContext) { }

    public async Task<List<OutboxItem>> GetNextUnprocessedItemsAsync(
        int batchSize,
        CancellationToken cancellationToken = default
    )
    {
        batchSize.MustBeIn(Range.InclusiveBetween(1, 1000));
        
        var dbContext = await GetDbContextAsync(cancellationToken);
        return await dbContext
           .OutboxItems
           .FromSqlInterpolated(
                $"""
                 SELECT * 
                 FROM outbox_items 
                 WHERE processed_at_utc IS NULL 
                 ORDER BY id 
                 LIMIT {batchSize} 
                 FOR UPDATE SKIP LOCKED
                 """
            )
           .ToListAsync(cancellationToken);
    }
}
