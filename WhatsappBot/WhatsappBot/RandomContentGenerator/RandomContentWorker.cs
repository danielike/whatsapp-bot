namespace WhatsappBot.RandomContentGenerator;

using Microsoft.Extensions.Options;
using ExternalApis.Evolution;
using Options;
using Microsoft.Extensions.Hosting;

public class RandomContentWorker : BackgroundService
{
    private readonly IRandomContentQueue _queue;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<ConfigurationOptions> _options;
    private readonly IRandomContentGenerator _randomContentGenerator;
    private readonly IServiceScopeFactory _scopeFactory;

    public RandomContentWorker(IRandomContentQueue queue, IHttpClientFactory httpClientFactory, IOptionsMonitor<ConfigurationOptions> options, IRandomContentGenerator randomContentGenerator,IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _httpClientFactory = httpClientFactory;
        _options = options;
        _randomContentGenerator = randomContentGenerator;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var item = await _queue.DequeueAsync(stoppingToken);
            
            try
            {
                // TODO: Get the content generator and send whatsapp message
                using var scope = _scopeFactory.CreateScope();
                var evolutionApiService = scope.ServiceProvider.GetRequiredService<IEvolutionApiService>();
                await evolutionApiService.SendMessage(_options.CurrentValue.EvolutionApiSendMessageId,  await _randomContentGenerator.Generate(item.Amount, item.Mention), item.Mention);
                // await ProcessItemAsync(item, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                // TODO: log ex (don’t crash the worker)
                // optionally implement retry/backoff here
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }

    // private async Task ProcessItemAsync(BackgroundWorkItem item, CancellationToken ct)
    // {
    //     var client = _httpClientFactory.CreateClient("external");
    //
    //     // Example HTTP call
    //     using var resp = await client.PostAsJsonAsync(
    //         "https://example.com/api/do-something",
    //         new { payload = item.Payload },
    //         ct);
    //
    //     resp.EnsureSuccessStatusCode();
    //
    //     // Perform additional actions...
    // }
}