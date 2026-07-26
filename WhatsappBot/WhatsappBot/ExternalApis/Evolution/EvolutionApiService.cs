using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using WhatsappBot.ExternalApis.Evolution.Responses;
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
        }
        
        return response;
    }

    public async Task<EvolutionApiTranscriberResponse> TranscribeAudio(string audioPath, string language)
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(language), "language");
        var fileStream = File.OpenRead(audioPath);
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/ogg");
        form.Add(streamContent, "file", Path.GetFileName(audioPath));
        
        var response = await _httpClient.PostAsync($"{_options.CurrentValue.EvolutionApiTranscriberEndpoint}", form);
        if (!response.IsSuccessStatusCode)
        {
            _logger.ErrorTranscribingAudio(response.StatusCode);
            return new EvolutionApiTranscriberResponse();
        }
        
        return await response.Content.ReadFromJsonAsync<EvolutionApiTranscriberResponse>() ?? new EvolutionApiTranscriberResponse();
    }
}
