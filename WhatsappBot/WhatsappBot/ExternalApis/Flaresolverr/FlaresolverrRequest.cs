namespace WhatsappBot.ExternalApis.Flaresolverr;

public class FlaresolverrRequest
{
    public string cmd { get; init; } = string.Empty;
    public string url { get; init; } = string.Empty;
    public int maxTimeout { get; init; }
}