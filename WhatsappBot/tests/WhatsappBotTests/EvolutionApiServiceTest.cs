using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moq.Protected;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.Options;

namespace WhatsappBotTests;
using Moq;

public class EvolutionApiServiceTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<IOptionsMonitor<ConfigurationOptions>>  _optionsMonitorMock = new();
    private readonly Mock<IEvolutionDelayCalculator> _evolutionDelayCalculatorMock = new();
    private readonly Mock<HttpMessageHandler> _mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    
    [Fact]
    public async Task ShouldSendMessage()
    {
        _optionsMonitorMock
            .Setup(property => property.CurrentValue)
            .Returns(new ConfigurationOptions
            {
                EvolutionApiInstance = "instance-test",
                EvolutionApiUrl = "http://localhost:8082",
                EvolutionApiEndpoint = "/message/sendText/",
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
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("apikey", $"{_optionsMonitorMock.Object.CurrentValue.EvolutionApiKey}");
                return httpClient;
            });
        
        _evolutionDelayCalculatorMock
            .Setup(calculator => calculator.GetHumanDelay(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(450);
        
        var evolutionApiService = new EvolutionApiService(_httpClientFactoryMock.Object, _evolutionDelayCalculatorMock.Object, _optionsMonitorMock.Object);
        
        var response = await evolutionApiService.SendMessage("test");
        
        Assert.True(response.IsSuccessStatusCode);
    }
}