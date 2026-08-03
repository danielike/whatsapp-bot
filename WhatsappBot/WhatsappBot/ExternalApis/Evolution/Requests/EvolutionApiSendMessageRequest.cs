namespace WhatsappBot.ExternalApis.Evolution.Requests;

using WhatsappBot.ExternalApis.Evolution;
public record EvolutionApiSendMessageRequest
{
    public string Number { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Delay { get; set; } = 2400;
}