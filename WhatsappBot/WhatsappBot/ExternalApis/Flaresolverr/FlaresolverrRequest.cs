namespace WhatsappBot.ExternalApis.Flaresolverr;

public class FlaresolverrRequest
{
    public string cmd { get; init; }
    public string url { get; init; }
    public int maxTimeout { get; init; }
}