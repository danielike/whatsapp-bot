namespace WhatsappBotTests;

using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.ExternalApis.Evolution.Responses;
using WhatsappBot.Options;
using Moq;

public class EvolutionApiServiceTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<IOptionsMonitor<ConfigurationOptions>>  _optionsMonitorMock = new();
    private readonly Mock<IEvolutionDelayCalculator> _evolutionDelayCalculatorMock = new();
    private readonly Mock<HttpMessageHandler> _mockHandler = new(MockBehavior.Strict);
    private readonly Mock<ILogger<EvolutionApiService>> _logger = new();
    
    [Fact]
    public async Task ShouldSendMessage()
    {
        _optionsMonitorMock
            .Setup(property => property.CurrentValue)
            .Returns(new ConfigurationOptions
            {
                EvolutionApiInstance = "instance-test",
                EvolutionApiUrl = "http://localhost:8082",
                EvolutionApiSendMessageEndpoint = "/message/sendText/",
                EvolutionApiKey = "key"
            });

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"status\":\"success\"}")
        };
        
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() =>
            {
                var httpClient = new HttpClient(_mockHandler.Object);
                httpClient.BaseAddress = new Uri(_optionsMonitorMock.Object.CurrentValue.EvolutionApiUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                httpClient.DefaultRequestHeaders.Add("apikey", $"{_optionsMonitorMock.Object.CurrentValue.EvolutionApiKey}");
                return httpClient;
            });
        
        _evolutionDelayCalculatorMock
            .Setup(calculator => calculator.GetHumanDelay(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(450);
        
        var evolutionApiService = new EvolutionApiService(_httpClientFactoryMock.Object, _evolutionDelayCalculatorMock.Object, _optionsMonitorMock.Object, _logger.Object);
        
        var response = await evolutionApiService.SendMessage("34600000000","test", "@12345678901");
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task ShouldTranscribeAudio()
    {
        _optionsMonitorMock
            .Setup(property => property.CurrentValue)
            .Returns(new ConfigurationOptions
            {
                EvolutionApiTranscriberUrl = "http://localhost:4040",
                EvolutionApiTranscriberEndpoint = "/transcribe",
                EvolutionApiTranscriberModel = "429683C4C977415CAAFCCE10F7D57E11"
            });

        var expectedResponse = new EvolutionApiTranscriberResponse { Transcription = "tanto." };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedResponse)
        };
        
        _mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() =>
            {
                var httpClient = new HttpClient(_mockHandler.Object);
                httpClient.BaseAddress = new Uri(_optionsMonitorMock.Object.CurrentValue.EvolutionApiTranscriberUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                httpClient.DefaultRequestHeaders.Add("apikey", $"{_optionsMonitorMock.Object.CurrentValue.EvolutionApiTranscriberModel}");
                return httpClient;
            });
        
        _evolutionDelayCalculatorMock
            .Setup(calculator => calculator.GetHumanDelay(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(450);
        
        var evolutionApiService = new EvolutionApiService(_httpClientFactoryMock.Object, _evolutionDelayCalculatorMock.Object, _optionsMonitorMock.Object, _logger.Object);

        var response = await evolutionApiService.TranscribeAudio("dW5rbm93bmNvbXBvc2l0aW9uY29udHJvbGVhc3lxdWlldGx5dGhyb3d0b29rZWR1Y2E=");
        
        Assert.Contains(expectedResponse.Transcription, response.Transcription);
    }
}