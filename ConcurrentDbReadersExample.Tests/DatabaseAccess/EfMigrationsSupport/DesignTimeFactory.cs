using System;
using Light.GuardClauses;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.EfMigrationsSupport;

public static class DesignTimeFactory
{
    public static TDbContext CreateDbContext<TDbContext>(
        string providerName,
        Func<string, ILogger, TDbContext> createDbContext,
        string[] args
    )
        where TDbContext : AppDbContext
    {
        var logger = DesignTimeCompositionRoot.CreateDesignTimeLogger();
        var configuration = DesignTimeCompositionRoot.CreateDesignTimeConfiguration(args);
        var connectionString = configuration.GetConnectionString(providerName);
        if (connectionString.IsNullOrWhiteSpace())
        {
            logger.Warning("There is no connection string for provider {ProviderName}", providerName);
            connectionString = string.Empty;
        }
        else
        {
            logger.Information(
                "Using connection string \"{ConnectionString}\" for provider {ProviderName}",
                connectionString,
                providerName
            );
        }

        return createDbContext(connectionString, logger);
    }
}
