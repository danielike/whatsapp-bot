using System.Net;

namespace WhatsappBot;

public static partial class WhatsappBotLogging
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Generator running at: {time}.")]
    public static partial void LogGeneratorRunningAt(this ILogger logger, DateTimeOffset time);
    
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Body was not valid JSON.")]
    public static partial void NotValidJson(this ILogger logger, Exception ex);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "EvolutionApi - Error sending message: {statusCode}.")]
    public static partial void ErrorSendingMessageEvolution(this ILogger logger, HttpStatusCode statusCode);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Error transcribing audio: {statusCode}.")]
    public static partial void ErrorTranscribingAudio(this ILogger logger, HttpStatusCode statusCode);
    
    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Getting selector.")]
    public static partial void ErrorGettingSelector(this ILogger logger, Exception ex);
    
    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "{content}")]
    public static partial void LogContentGenerated(this ILogger logger, string content);
    
    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Getting flaresolverr solution:\n {message}")]
    public static partial void ErrorGettingFlaresolverrSolution(this ILogger logger, string message);
    
    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Executing random content worker.")]
    public static partial void ErrorRandomContentWorkerExecuteAsync(this ILogger logger, Exception ex);
    
    [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "Allowed Jids: {allowedJids}.")]
    public static partial void ShowAllowedJids(this ILogger logger, string allowedJids);
}