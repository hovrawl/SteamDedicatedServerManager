namespace SteamDedicatedServerManager.Services;

public interface IConsoleService
{
    Task SendMessage(string message, bool error = false);
}