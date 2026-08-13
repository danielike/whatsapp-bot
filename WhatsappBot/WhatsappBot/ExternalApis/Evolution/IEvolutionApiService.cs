namespace WhatsappBot.ExternalApis.Evolution;

using Responses;

public interface IEvolutionApiService
{
    Task<HttpResponseMessage> SendMessage(string number, string message, string mention);
    Task<EvolutionApiTranscriberResponse> TranscribeAudio(string audioPath, string language);
}