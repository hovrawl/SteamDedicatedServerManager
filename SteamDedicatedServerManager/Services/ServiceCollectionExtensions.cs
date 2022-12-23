namespace SteamDedicatedServerManager.Services;

internal static class ServiceCollectionExtensions
{
    #region Fields
    private const string CONSOLE_SERVICE_TYPE_CONFIGURATION_KEY = "ConsoleService";
    private const string CONSOLE_SERVICE_TYPE_DOWNLOAD = "Download";
    private const string CONSOLE_SERVICE_TYPE_SERVER = "Server";
    #endregion
    
    public static IServiceCollection AddConsoleService(this IServiceCollection services, IConfiguration configuration)
    {
        string notificationsServiceType = configuration.GetValue(CONSOLE_SERVICE_TYPE_CONFIGURATION_KEY, CONSOLE_SERVICE_TYPE_DOWNLOAD);

        if (notificationsServiceType.Equals(CONSOLE_SERVICE_TYPE_DOWNLOAD, StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddTransient<IConsoleService, ConsoleService>();
        }
        else if (notificationsServiceType.Equals(CONSOLE_SERVICE_TYPE_SERVER, StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddSingleton<IConsoleService, ConsoleService>();
        }
        else
        {
            throw new NotSupportedException($"Not supported {nameof(IConsoleService)} type.");
        }

        return services;
    }
}