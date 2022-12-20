namespace VRisingServerManagement.Services;

public interface IConsoleService
{
    Task SendMessage(string message, bool error = false);
}