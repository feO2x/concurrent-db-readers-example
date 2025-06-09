using System;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess;

public sealed class OutboxItem
{
    public required Guid Id { get; init; }
    
    public required string Type { get; init; }
    
    public required string JsonPayload { get; init; }
    
    public required DateTime CreatedAtUtc { get; init; }
    
    public DateTime? ProcessedAtUtc { get; init; }
    
    public string? ProcessedBy { get; init; }
}