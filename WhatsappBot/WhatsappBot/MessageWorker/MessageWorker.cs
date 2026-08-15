namespace WhatsappBot.MessageWorker;

using RandomContentGenerator;
using Microsoft.Extensions.Options;
using ExternalApis.Evolution;
using Options;
using Microsoft.Extensions.Hosting;

public class MessageWorker : BackgroundService
{
    private readonly IMessageQueue _queue;
    private readonly IOptionsMonitor<ConfigurationOptions> _options;
    private readonly IRandomContentGenerator _randomContentGenerator;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageWorker> _logger;

    public MessageWorker(IMessageQueue queue, IOptionsMonitor<ConfigurationOptions> options, IRandomContentGenerator randomContentGenerator, IServiceScopeFactory scopeFactory, ILogger<MessageWorker> logger)
    {
        _queue = queue;
        _options = options;
        _randomContentGenerator = randomContentGenerator;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var item = await _queue.DequeueAsync(stoppingToken);
            
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var evolutionApiService = scope.ServiceProvider.GetRequiredService<IEvolutionApiService>();
                
                if (item.Amount != 0)
                {
                    await evolutionApiService.SendMessage(_options.CurrentValue.EvolutionApiSendMessageId,  await _randomContentGenerator.Generate(item.Amount, item.Mention), item.Mention);
                }

                if (!string.IsNullOrEmpty(item.Base64Audio))
                {
                    await evolutionApiService.SendMessage(_options.CurrentValue.EvolutionApiSendMessageId, $"{item.Mention}\n{(await evolutionApiService.TranscribeAudio(item.Base64Audio)).Transcription}", item.Mention);
                }

            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.ErrorRandomContentWorkerExecuteAsync(ex);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}