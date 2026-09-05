namespace WhatsappBot.Command;

using Microsoft.Extensions.Options;
using Options;
using ExternalApis.Evolution;
using System.Collections.Concurrent;

public class BuzzService : IBuzzService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeBuzzes = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;

    public BuzzService(IServiceScopeFactory scopeFactory, IOptionsMonitor<ConfigurationOptions> configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }
    
    public Guid StartBuzz(
        TimeSpan interval,
        string id,
        string mention)
    {
        var guid = Guid.NewGuid();
        var cts = new CancellationTokenSource();

        _activeBuzzes[guid] = cts;

        _ = RunBuzzAsync(guid, interval, id, mention, cts);

        return guid;
    }

    private async Task RunBuzzAsync(
        Guid guid,
        TimeSpan interval,
        string id,
        string mention,
        CancellationTokenSource cts)
    {
        try
        {
            using var timer = new PeriodicTimer(interval);
            
            using var scope = _scopeFactory.CreateScope();
            var evolutionApiService = scope.ServiceProvider.GetRequiredService<IEvolutionApiService>();

            while (await timer.WaitForNextTickAsync(cts.Token))
            {
                await evolutionApiService.SendMessage(id, $"{mention}\n{_configuration.CurrentValue.BuzzMessage}", mention);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when /stop is called.
        }
        finally
        {
            _activeBuzzes.TryRemove(guid, out _);
            cts.Dispose();
        }
    }

    public void StopAll()
    {
        foreach (var cts in _activeBuzzes.Values)
        {
            cts.Cancel();
        }
    }

    public void Stop(Guid id)
    {
        if (_activeBuzzes.TryGetValue(id, out var cts))
        {
            cts.Cancel();
        }
    }
}