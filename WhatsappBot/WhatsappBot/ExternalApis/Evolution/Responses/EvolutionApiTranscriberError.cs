using System.Text.Json.Serialization;

namespace WhatsappBot.ExternalApis.Evolution.Responses;

public record EvolutionApiTranscriberError
{
    [JsonPropertyName("detail")]
    public string Detail { get; init; } = string.Empty;
}