namespace WhatsappBot;

public static partial class WhatsappBotLogging
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Background job running at: {time}")]
    public static partial void LogBackgroundJobRunningAt(this ILogger logger, DateTimeOffset time);
}