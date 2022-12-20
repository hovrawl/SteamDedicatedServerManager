namespace VRisingServerManagement.Services;

internal class ConsoleService: ConsoleServiceBase, IConsoleService
{
    #region Constructor
    public ConsoleService(IConsoleServerSentEventService notificationsServerSentEventsService)
        : base(notificationsServerSentEventsService)
    { }
    #endregion

    #region Methods
    public Task SendMessage(string notification, bool alert)
    {
        return SendSseEventAsync(notification, alert);
    }
    #endregion
}