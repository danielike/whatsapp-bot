#!/usr/bin/env dotnet-script
                                        
#r "nuget: AngleSharp, 1.4.0"

#nullable enable

using AngleSharp;
using AngleSharp.Dom;
using System.Net.Http;
using System.Threading.Tasks;

var forbiddenGenres = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
{
    "Yaoi",
    "Futanari",
    "Gore"
}

async Task<(string? Key, IEnumerable<string?> Values)> GetContentByCssSelectorAsync(string url, string nameSelector, string genresSelector) 
{
    var config = Configuration.Default.WithDefaultLoader();
    var context = BrowsingContext.New(config);
    var document = await context.OpenAsync(url);
    var name = document.QuerySelector(nameSelector);
    var genres = document.QuerySelectorAll(genresSelector);
    var keyName = name!.GetAttribute("content");
    var values = genres!.Select(m => m.GetAttribute("content"));
    return (keyName, values);
}

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
            tasks[i] = GetContentByCssSelectorAsync("https://muchohentai.com/random-video/", "meta[property=\"article:section\"]", "meta[property=\"article:tag\"]");
        }
        contents = await Task.WhenAll(tasks);
    }
    catch(Exception e)
    {
        Console.WriteLine($"ERROR - Getting selector. Ex: {e}");
        continue;
    }
    Console.WriteLine($"{ string.Join(",\n", contents.SelectMany(genres => genres.Values ?? Enumerable.Empty<string?>()).Select(genre => genre ?? string.Empty ) ) }");
    await Task.Delay(TimeSpan.FromSeconds(5));
}
