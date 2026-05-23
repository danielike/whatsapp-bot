#!/usr/bin/env dotnet-script
                                        
#r "nuget: AngleSharp, 1.4.0"

#nullable enable

using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var forbiddenGenres = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
{
    "Yaoi",
    "Futanari",
    "Gore"
};

async Task<string> GetHtml(string siteUrl)
{
    var client = new HttpClient();
    var flareSolverrUrl = "http://localhost:8191/v1";

    var payload = new
    {
        cmd = "request.get",
        url = siteUrl,
        maxTimeout = 60000
    };

    var json = JsonSerializer.Serialize(payload);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");

    try
    {
        var response = await client.PostAsync(flareSolverrUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        string status = root.TryGetProperty("status", out var s) ? s.ToString() : "";
        string message = root.TryGetProperty("message", out var m) ? m.ToString() : "";
        string solutionResponse = "";

        if (root.TryGetProperty("solution", out var sol) &&
            sol.TryGetProperty("response", out var resp))
        {
            solutionResponse = resp.GetString() ?? resp.ToString();
        }
        
        return solutionResponse;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error Getting FlareSolverr Solution: " + ex.Message);
    }
    return string.Empty;
}

async Task<(string? Key, IEnumerable<string?> Values)> GetContentByCssSelectorAsync(string html, string nameSelector, string genresSelector) 
{
    var config = Configuration.Default.WithDefaultLoader();
    var context = BrowsingContext.New(config);
    var document = await context.OpenAsync(req => req.Content(@html));
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
            tasks[i] = GetContentByCssSelectorAsync(await GetHtml("https://muchohentai.com/random-video/"), "meta[property=\"article:section\"]", "meta[property=\"article:tag\"]");
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
