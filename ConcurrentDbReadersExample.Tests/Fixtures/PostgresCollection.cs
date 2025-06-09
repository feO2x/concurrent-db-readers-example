using Xunit;

namespace ConcurrentDbReadersExample.Tests.Fixtures;

[CollectionDefinition(nameof(PostgresCollection), DisableParallelization = true)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>;
