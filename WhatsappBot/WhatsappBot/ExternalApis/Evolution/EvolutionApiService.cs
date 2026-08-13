namespace WhatsappBot.ExternalApis.Evolution;

using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Requests;
using Responses;
using Options;

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
        
        var response = await _httpClient.PostAsJsonAsync($"{_options.CurrentValue.EvolutionApiSendMessageEndpoint}/{_options.CurrentValue.EvolutionApiInstance}", request);
        
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
