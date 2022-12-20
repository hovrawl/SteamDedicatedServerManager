using Lib.AspNetCore.ServerSentEvents;

namespace VRisingServerManagement.Services;

internal abstract class ConsoleServiceBase
{
    #region Fields
    private IConsoleServerSentEventService _consoleServerSentEventService;
    #endregion

    #region Constructor
    protected ConsoleServiceBase(IConsoleServerSentEventService consoleServerSentEventService)
    {
        _consoleServerSentEventService = consoleServerSentEventService;
    }
    #endregion

    #region Methods
    protected Task SendSseEventAsync(string message, bool error)
    {
        return _consoleServerSentEventService.SendEventAsync(new ServerSentEvent
        {
            Type = error ? "error" : null,
            Data = new List<string>(message.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
        });
    }
    #endregion
}