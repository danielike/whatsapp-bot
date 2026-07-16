namespace WhatsappBot.ExternalApis.Evolution.Responses;

public record EvolutionApiTranscriberResponse
{
    public string Transcription { get; init; } = string.Empty;
}