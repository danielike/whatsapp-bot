using System.Text.Json;
using Microsoft.Extensions.Options;
using WhatsappBot.Options;

namespace WhatsappBot.ExternalApis.Evolution;

public class EvolutionApiService : IEvolutionApiService
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<ConfigurationOptions> _options;
    
    public EvolutionApiService(IHttpClientFactory httpClientFactory, IOptionsMonitor<ConfigurationOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(EvolutionApiService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    public async Task<HttpResponseMessage> SendMessage(string message)
    {
        // ArgumentNullException.ThrowIfNull(configuration);
        // TODO: curl -X POST http://localhost:8082/message/sendText/Bot with apikey => evolutionApiKey in header
        // TODO: request => { "text" : "test", "delay": 1200, "mentioned":{"{{remoteJid}}"} }
        var request = new
        {
            text = message,
            delay = EvolutionDelayCalculator.GetHumanDelay(message)
        };
        
        var response = await _httpClient.PostAsJsonAsync($"{_options.CurrentValue.EvolutionApiEndpoint}/{_options.CurrentValue.EvolutionApiInstance}", JsonSerializer.Serialize(request));
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error sending message: {response.StatusCode}");
        }

        return response;
    }

    public Task<HttpResponseMessage> TranscribeAudio(string audioPath, string language)
    {
        throw new NotImplementedException();
    }
}