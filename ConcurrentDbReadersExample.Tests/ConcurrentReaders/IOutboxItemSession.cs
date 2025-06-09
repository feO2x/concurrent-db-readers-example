using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.DatabaseAccess;
using Light.SharedCore.DatabaseAccessAbstractions;

namespace ConcurrentDbReadersExample.Tests.ConcurrentReaders;

public interface IOutboxItemSession : ISession
{
    Task<List<OutboxItem>> GetNextUnprocessedItemsAsync(int batchSize, CancellationToken cancellationToken = default);
}