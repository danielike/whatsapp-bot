using System.Text.Json;
using Microsoft.Extensions.Options;
using WhatsappBot.Options;

namespace WhatsappBot.ExternalApis.Evolution;

public class EvolutionApiService : IEvolutionApiService
{
    private readonly HttpClient _httpClient;
    private readonly IEvolutionDelayCalculator _evolutionDelayCalculator;
    private readonly IOptionsMonitor<ConfigurationOptions> _options;
    private readonly ILogger<EvolutionApiService> _logger;
    
    public EvolutionApiService(IHttpClientFactory httpClientFactory, IEvolutionDelayCalculator evolutionDelayCalculator, IOptionsMonitor<ConfigurationOptions> options, ILogger<EvolutionApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(EvolutionApiService));
        _evolutionDelayCalculator = evolutionDelayCalculator;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }
    
    public async Task<HttpResponseMessage> SendMessage(string message)
    {
        // ArgumentNullException.ThrowIfNull(configuration);
        // TODO: curl -X POST http://localhost:8082/message/sendText/Bot with apikey => evolutionApiKey in header
        // TODO: request => { "text" : "test", "delay": 1200, "mentioned":{"{{remoteJid}}"} }
        var request = new
        {
            text = message,
            delay = _evolutionDelayCalculator.GetHumanDelay(message)
        };
        
        var response = await _httpClient.PostAsJsonAsync($"{_options.CurrentValue.EvolutionApiEndpoint}/{_options.CurrentValue.EvolutionApiInstance}", request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.ErrorSendingMessageEvolution(response.StatusCode);
            Console.WriteLine($"Error sending message: {response.StatusCode}");
        }

        return response;
    }

    public Task<HttpResponseMessage> TranscribeAudio(string audioPath, string language)
    {
        throw new NotImplementedException();
    }
}