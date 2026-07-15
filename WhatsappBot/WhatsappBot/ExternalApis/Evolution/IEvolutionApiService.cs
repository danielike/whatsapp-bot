namespace WhatsappBot.ExternalApis.Evolution;

public interface IEvolutionApiService
{
    Task<HttpResponseMessage> SendMessage(string message);
    Task<HttpResponseMessage> TranscribeAudio(string audioPath, string language);
}