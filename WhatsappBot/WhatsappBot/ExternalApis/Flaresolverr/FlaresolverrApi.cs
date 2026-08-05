namespace WhatsappBot.ExternalApis.Flaresolverr;

using System.Text;
using System.Text.Json;
using System.Net.Mime;

public class FlaresolverrApi : IFlaresolverrApi
{
    private readonly HttpClient _client;
    private readonly ILogger<FlaresolverrApi> _logger;
    public FlaresolverrApi(IHttpClientFactory httpClientFactory, ILogger<FlaresolverrApi> logger)
    {
        _client = httpClientFactory.CreateClient(nameof(FlaresolverrApi));
        _logger = logger;
    }
    
    public async Task<string> GetHtml(string flareSolverrUrl, string siteUrl, CancellationToken token = default)
    {
        var payload = new FlaresolverrRequest
        {
            cmd = "request.get",
            url = siteUrl,
            maxTimeout = 60000
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        try
        {
            var response = await _client.PostAsync(flareSolverrUrl, content, token);
            
            var responseBody = await response.Content.ReadAsStringAsync(token);

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            
            string solutionResponse = "";

            if (root.TryGetProperty("solution", out var sol) &&
                sol.TryGetProperty("response", out var resp))
            {
                solutionResponse = resp.GetString() ?? resp.ToString();
            }
    
            return solutionResponse;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.ErrorGettingFlaresolverrSolution(ex.Message);
            return string.Empty;
        }
    }
}