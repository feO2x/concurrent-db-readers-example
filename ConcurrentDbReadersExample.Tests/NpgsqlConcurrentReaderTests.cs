using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.ConcurrentReaders;
using ConcurrentDbReadersExample.Tests.Fixtures;
using FluentAssertions;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConcurrentDbReadersExample.Tests;

[Collection(nameof(PostgresCollection))]
public sealed class NpgsqlConcurrentReaderTests
{
    private const int BatchSize = 13;
    private readonly PostgresFixture _fixture;

    public NpgsqlConcurrentReaderTests(PostgresFixture fixture, ITestOutputHelper testOutput)
    {
        _fixture = fixture;
        _fixture.TestOutputSink.Inject(testOutput);
    }

    [Fact]
    public async Task PerformPostgresConcurrentReaderTests()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await PrepareOutboxItemsAsync(100, cancellationToken);
        var processor1 = CreateOutboxItemProcessor("P1");
        var processor2 = CreateOutboxItemProcessor("P2");
        var processor3 = CreateOutboxItemProcessor("P3");

        // Act
        await Task.WhenAll(
            processor1.ProcessOutboxItemsAsync(cancellationToken),
            processor2.ProcessOutboxItemsAsync(cancellationToken),
            processor3.ProcessOutboxItemsAsync(cancellationToken)
        );

        // Assert
        await using var dbContext = _fixture.CreateDbContext();
        var allOutboxItems = await dbContext.OutboxItems.AsNoTracking().ToListAsync(cancellationToken);
        allOutboxItems.Should().NotContain(x => x.ProcessedBy.IsNullOrEmpty(), "All outbox items should be processed");
        var groups = allOutboxItems.GroupBy(x => x.ProcessedBy).ToDictionary(x => x.Key!, x => x.Count());
        foreach (var outboxItem in allOutboxItems)
        {
            _fixture.Logger.Information(
                "Outbox item {OutboxItemId} was processed by {ProcessedBy}",
                outboxItem.Id,
                outboxItem.ProcessedBy
            );
        }

        var countP1 = groups["P1"];
        var countP2 = groups["P2"];
        var countP3 = groups["P3"];
        _fixture.Logger.Information("P1 processed {OutboxItemCount} outbox items in total", countP1);
        _fixture.Logger.Information("P2 processed {OutboxItemCount} outbox items in total", countP2);
        _fixture.Logger.Information("P3 processed {OutboxItemCount} outbox items in total", countP3);
        var minimum = Math.Min(countP1, Math.Min(countP2, countP3));
        minimum.Should().BeGreaterOrEqualTo(
            BatchSize,
            "The processor with the least amount of outbox items should have processed at least one batch"
        );
    }

    private async Task PrepareOutboxItemsAsync(int numberOfItems, CancellationToken cancellationToken = default)
    {
        numberOfItems.MustBeGreaterThan(0);

        await using var dbContext = _fixture.CreateDbContext();
        await dbContext.OutboxItems.ExecuteDeleteAsync(cancellationToken);
        var now = DateTime.UtcNow;
        for (var i = 0; i < numberOfItems; i++)
        {
            dbContext.OutboxItems.Add(
                new ()
                {
                    Id = Guid.CreateVersion7(),
                    CreatedAtUtc = now,
                    JsonPayload = $"{{\"message\":\"Hello {i + 1}\"}}",
                    Type = "testMessage"
                }
            );
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private OutboxItemProcessor CreateOutboxItemProcessor(string processorId) =>
        new (
            processorId,
            BatchSize,
            () => new NpgsqlOutboxItemsSession(_fixture.CreateDbContext()),
            _fixture.Logger
        );
}
