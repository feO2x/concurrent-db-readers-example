using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentDbReadersExample.Tests.DatabaseAccess;
using Light.DatabaseAccess.EntityFrameworkCore;
using Light.GuardClauses;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Range = Light.GuardClauses.Range;

namespace ConcurrentDbReadersExample.Tests.ConcurrentReaders;

public sealed class OracleOutboxItemsSession : EfSession<OracleAppDbContext>.WithTransaction, IOutboxItemSession
{
    public OracleOutboxItemsSession(OracleAppDbContext dbContext) : base(dbContext) { }

    public async Task<List<OutboxItem>> GetNextUnprocessedItemsAsync(
        int batchSize,
        CancellationToken cancellationToken = default
    )
    {
        batchSize.MustBeIn(Range.InclusiveBetween(1, 1000));

        const string sql =
            """
            BEGIN
                OPEN :result_cursor FOR
                    SELECT ID, TYPE, JSON_PAYLOAD, CREATED_AT_UTC, PROCESSED_AT_UTC, PROCESSED_BY
                    FROM OUTBOX_ITEMS
                    WHERE PROCESSED_AT_UTC IS NULL
                    ORDER BY ID
                    FOR UPDATE SKIP LOCKED;
            END;
            """;

        var command = await CreateCommandAsync<OracleCommand>(sql, cancellationToken);

        // Add the cursor parameter
        var cursorParameter = new OracleParameter("result_cursor", OracleDbType.RefCursor)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(cursorParameter);

        // Execute the PL/SQL block
        await command.ExecuteNonQueryAsync(cancellationToken);

        var results = new List<OutboxItem>();

        // Get the cursor from the output parameter and read from it
        var refCursor = (OracleRefCursor) cursorParameter.Value!;
        await using var reader = refCursor.GetDataReader();

        // Only read up to batchSize rows - this is where the batching happens
        var rowsRead = 0;
        while (rowsRead < batchSize && await reader.ReadAsync(cancellationToken))
        {
            var outboxItem = new OutboxItem
            {
                Id = reader.GetGuid(0), // ID
                Type = reader.GetString(1), // TYPE
                JsonPayload = reader.GetString(2), // JSON_PAYLOAD
                CreatedAtUtc = reader.GetDateTime(3), // CREATED_AT_UTC
                ProcessedAtUtc = reader.IsDBNull(4) ? null : reader.GetDateTime(4), // PROCESSED_AT_UTC
                ProcessedBy = reader.IsDBNull(5) ? null : reader.GetString(5) // PROCESSED_BY
            };

            results.Add(outboxItem);
            DbContext.OutboxItems.Attach(outboxItem);
            rowsRead++;
        }

        return results;
    }
}
