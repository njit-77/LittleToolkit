using Microsoft.Extensions.DependencyInjection;

namespace Hash.Extensions;

public static class LoggingExtensions
{
    public static void AddLogger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<Log.ILogger, Log.Logger>();
    }
}
