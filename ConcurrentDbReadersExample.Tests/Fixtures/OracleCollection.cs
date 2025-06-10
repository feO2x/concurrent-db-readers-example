using Xunit;

namespace ConcurrentDbReadersExample.Tests.Fixtures;

[CollectionDefinition(nameof(OracleCollection), DisableParallelization = true)]
public sealed class OracleCollection : ICollectionFixture<OracleFixture>;
