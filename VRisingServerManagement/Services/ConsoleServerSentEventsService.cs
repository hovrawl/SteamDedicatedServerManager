using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace VRisingServerManagement.Services;

public class ConsoleServerSentEventService : ServerSentEventsService, IConsoleServerSentEventService
{
    public ConsoleServerSentEventService(IOptions<ServerSentEventsServiceOptions<ConsoleServerSentEventService>> options)
        : base(options.ToBaseServerSentEventsServiceOptions())
    { }
}