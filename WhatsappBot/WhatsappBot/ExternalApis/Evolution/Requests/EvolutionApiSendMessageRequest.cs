namespace WhatsappBot.ExternalApis.Evolution.Requests;

public record EvolutionApiSendMessageRequest
{
    public string Number { get; set; } = string.Empty;
    public bool MentionsEveryone { get; set; } = false;
    public string[] Mentioned { get; set; } = [];
    public string Text { get; set; } = string.Empty;
    public int Delay { get; set; } = 2400;
}