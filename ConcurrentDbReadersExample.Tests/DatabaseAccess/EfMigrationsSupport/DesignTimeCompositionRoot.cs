using Microsoft.Extensions.Configuration;
using Serilog;

namespace ConcurrentDbReadersExample.Tests.DatabaseAccess.EfMigrationsSupport;

public static class DesignTimeCompositionRoot
{
    public static IConfiguration CreateDesignTimeConfiguration(string[] args) =>
        new ConfigurationBuilder()
           .AddJsonFile("appsettings.jsonc", true)
           .AddUserSecrets(typeof(NpgsqlAppDbContextDesignTimeFactory).Assembly, true)
           .AddCommandLine(args)
           .Build();

    public static ILogger CreateDesignTimeLogger() =>
        new LoggerConfiguration()
           .WriteTo.Console()
           .CreateLogger();
}