using System;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;
using Serilog;
using Range = Light.GuardClauses.Range;

namespace ConcurrentDbReadersExample.Tests.ConcurrentReaders;

public sealed class OutboxItemProcessor
{
    private readonly int _batchSize;
    private readonly Func<IOutboxItemSession> _createSession;
    private readonly ILogger _logger;
    private readonly string _processorId;

    public OutboxItemProcessor(
        string processorId,
        int batchSize,
        Func<IOutboxItemSession> createSession,
        ILogger logger
    )
    {
        _processorId = processorId.MustNotBeNullOrWhiteSpace();
        _batchSize = batchSize.MustBeIn(Range.InclusiveBetween(1, 1000));
        _createSession = createSession;
        _logger = logger;
    }

    public async Task ProcessOutboxItemsAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var now = DateTime.UtcNow;
            await using var session = _createSession();
            var outboxItems = await session.GetNextUnprocessedItemsAsync(_batchSize, cancellationToken);

            if (outboxItems.Count is 0)
            {
                return;
            }

            foreach (var outboxItem in outboxItems)
            {
                outboxItem.ProcessedAtUtc = now;
                outboxItem.ProcessedBy = _processorId;
            }

            await session.SaveChangesAsync(cancellationToken);
            _logger.Information("Processed {OutboxItemsCount} outbox items", outboxItems.Count);
        }
    }
}
