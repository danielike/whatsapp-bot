namespace WhatsappBot.ExternalApis.Evolution;

using Common;
using Microsoft.Extensions.Options;
using Requests;
using Responses;
using Options;

public class EvolutionApiService : IEvolutionApiService
{
    private readonly HttpClient _evolutionApiClient;
    private readonly HttpClient _evolutionApiTranscriberClient;
    private readonly IEvolutionDelayCalculator _evolutionDelayCalculator;
    private readonly IOptionsMonitor<ConfigurationOptions> _options;
    private readonly ILogger<EvolutionApiService> _logger;
    
    public EvolutionApiService(IHttpClientFactory httpClientFactory, IEvolutionDelayCalculator evolutionDelayCalculator, IOptionsMonitor<ConfigurationOptions> options, ILogger<EvolutionApiService> logger)
    {
        _evolutionApiClient = httpClientFactory.CreateClient(nameof(EvolutionApiService));
        _evolutionApiTranscriberClient = httpClientFactory.CreateClient(HttpClientNames.EvolutionApiTranscriber); 
        _evolutionDelayCalculator = evolutionDelayCalculator;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }
    
    public async Task<HttpResponseMessage> SendMessage(string number, string message, string mention)
    {
        var request = new EvolutionApiSendMessageRequest
        {
            Number = number,
            MentionsEveryone = false,
            Mentioned = [$"{mention.Remove(0, 1)}@lid"],
            Text = message,
            Delay = _evolutionDelayCalculator.GetHumanDelay(message)
        };
        
        var response = await _evolutionApiClient.PostAsJsonAsync($"{_options.CurrentValue.EvolutionApiSendMessageEndpoint}/{_options.CurrentValue.EvolutionApiInstance}", request);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.ErrorSendingMessageEvolution(response.StatusCode);
        }
        
        return response;
    }

    public async Task<EvolutionApiTranscriberResponse> TranscribeAudio(string base64Audio)
    {
        using var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent($"data:audio/ogg;base64,{base64Audio}"), "url");
        formData.Add(new StringContent("whisper-large-v3-turbo"), "model");
        
        var response = await _evolutionApiTranscriberClient.PostAsync(_options.CurrentValue.EvolutionApiTranscriberEndpoint, formData);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.ErrorTranscribingAudio(response.StatusCode);
            return new EvolutionApiTranscriberResponse();
        }
        
        return await response.Content.ReadFromJsonAsync<EvolutionApiTranscriberResponse>() ?? new EvolutionApiTranscriberResponse();
    }
}
