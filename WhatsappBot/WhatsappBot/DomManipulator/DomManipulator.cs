namespace WhatsappBot.DomManipulator;

using AngleSharp;

public class DomManipulator : IDomManipulator
{
    public async Task<(string? Key, IEnumerable<string?> Values)> GetContentByCssSelectorAsync(
        string html, 
        string nameSelector, 
        string genresSelector, 
        HashSet<string> forbiddenGenres)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));
        var name = document.QuerySelector(nameSelector);
        var genres = document.QuerySelectorAll(genresSelector);
        var keyName = name!.GetAttribute("content");
        var values = genres.Select(m => m.GetAttribute("content")).ToList();
        bool containsForbiddenGenres = values.Any(genre => forbiddenGenres.Contains(genre!));
        return containsForbiddenGenres ? (string.Empty, []) : (keyName, values);
    }
}