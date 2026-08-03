using WhatsappBot.ExternalApis.Evolution.Responses;

namespace WhatsappBot.ExternalApis.Evolution;

public interface IEvolutionApiService
{
    Task<HttpResponseMessage> SendMessage(string number, string message);
    Task<EvolutionApiTranscriberResponse> TranscribeAudio(string audioPath, string language);
}