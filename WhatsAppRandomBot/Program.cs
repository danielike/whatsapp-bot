using dotenv.net;
using WhatsAppRandomBot;
using WhatsAppRandomBot.ExternalApis;

DotEnv.Load();
string forbidden = Environment.GetEnvironmentVariable("FORBIDDEN_GENRES") ?? throw new ArgumentNullException($"FORBIDDEN_GENRES not in .env");
string flareSolverrUrl = Environment.GetEnvironmentVariable("FLARESOLVERR_URL") ?? throw new ArgumentNullException($"FLARESOLVERR_URL not in .env");
string siteUrlEnv = Environment.GetEnvironmentVariable("SITE_URL") ?? throw new ArgumentNullException($"SITE_URL not in .env");

var forbiddenGenres = new HashSet<string>( 
forbidden
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

var handler = new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
};

var client = new HttpClient(handler);

while(true)
{
    // TODO: Listen whatsapp messages using Evolution API
    // TODO: If message is: /random n, start to process. If not, ignore it.
    string? text = null;
    
    // TODO:  Change by n size based on: /random n
    int n = 5;
    var contents = new (string? Key, IEnumerable<string?> Values)[n];
    var tasks = new Task<(string? Key, IEnumerable<string?> Values)>[n];
    try
    {
        for(int i = 0; i < n; i++)
        {
            // TODO: ignore any name that contains forbidden genres. Where(genre => !string.IsNullOrEmpty() && forbiddenGenres.Contains(genre))
            tasks[i] = DomManipulator.GetContentByCssSelectorAsync(await FlaresolverrApi.GetHtml(client, flareSolverrUrl, siteUrlEnv), "meta[property=\"article:section\"]", "meta[property=\"article:tag\"]");
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
    await Task.Delay(TimeSpan.FromSeconds(5));
}



