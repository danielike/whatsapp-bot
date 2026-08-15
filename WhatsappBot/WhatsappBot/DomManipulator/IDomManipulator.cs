namespace WhatsappBot.DomManipulator;

using System.Buffers;

public interface IDomManipulator
{
    Task<(string? Key, IEnumerable<string?> Values)> GetContentByCssSelectorAsync(string html, string nameSelector,
        string genresSelector, SearchValues<string> forbiddenGenres);
}