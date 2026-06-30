using Microsoft.Extensions.Options;
using WhatsappBot.ExternalApis.Flaresolverr;
using WhatsappBot.Options;

namespace WhatsappBot;

public class GenerateRandomContent(
    ILogger<GenerateRandomContent> logger, 
    IOptionsMonitor<ConfigurationOptions> configuration
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var forbiddenGenres = new HashSet<string>( 
            configuration.CurrentValue.ForbiddenGenres
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
        );
        /*
        "Element1",
        "Element2",
        "Element3",
        ...
        */

        // TODO: Refactor: inject into constructor
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
        };

        var client = new HttpClient(handler);
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogBackgroundJobRunningAt(DateTimeOffset.Now);
            
            // TODO: Listen whatsapp messages using Evolution API
            // TODO: If message is: /random n @mention, start to process. If not, ignore it.
            string? text = null;
    
            // TODO:  Change by n size based on: /random n @mention
            int n = 5;
            var contents = new (string? Key, IEnumerable<string?> Values)[n];
            var tasks = new Task<(string? Key, IEnumerable<string?> Values)>[n];
            try
            {
                for(int i = 0; i < n; i++)
                {
                    // TODO: ignore any name that contains forbidden genres. Where(genre => !string.IsNullOrEmpty() && forbiddenGenres.Contains(genre))
                    tasks[i] = DomManipulator.GetContentByCssSelectorAsync(await FlaresolverrApi.GetHtml(client, configuration.CurrentValue.FlaresolverrUrl, configuration.CurrentValue.SiteUrl), configuration.CurrentValue.NameSelector, configuration.CurrentValue.GenresSelector);
                }
                contents = await Task.WhenAll(tasks);
            }
            catch(Exception e)
            {
                Console.WriteLine($"ERROR - Getting selector. Ex: {e}");
                continue;
            }
            Console.Write($"{string.Join(",", contents.Select(m => m.Key))} => ");
            Console.WriteLine($"{ string.Join(",\n", contents.SelectMany(genres => genres.Values ?? Enumerable.Empty<string?>()).Select(genre => genre ?? string.Empty ) ) }");
            await Task.Delay(5000, stoppingToken); // Wait for 5 seconds
        }
    }
}