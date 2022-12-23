using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace SteamDedicatedServerManager.Services;

public class ConsoleServerSentEventService : ServerSentEventsService, IConsoleServerSentEventService
{
    public ConsoleServerSentEventService(IOptions<ServerSentEventsServiceOptions<ConsoleServerSentEventService>> options)
        : base(options.ToBaseServerSentEventsServiceOptions())
    { }
}