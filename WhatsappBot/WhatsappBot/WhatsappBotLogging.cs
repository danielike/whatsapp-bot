using System.Net;

namespace WhatsappBot;

public static partial class WhatsappBotLogging
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Background job running at: {time}")]
    public static partial void LogBackgroundJobRunningAt(this ILogger logger, DateTimeOffset time);
    
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Body was not valid JSON.")]
    public static partial void NotValidJson(this ILogger logger, Exception ex);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "EvolutionApi - Error sending message: {statusCode}.")]
    public static partial void ErrorSendingMessageEvolution(this ILogger logger, HttpStatusCode statusCode);
}