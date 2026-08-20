using System.Text.Json.Serialization;

namespace WhatsappBot.ExternalApis.Evolution.Responses;

public record EvolutionApiTranscriberResponse
{
    [JsonPropertyName("text")]
    public string Transcription { get; init; } = string.Empty;
}