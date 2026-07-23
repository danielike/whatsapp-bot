namespace WhatsappBot.DomManipulator;

public interface IDomManipulator
{
    Task<(string? Key, IEnumerable<string?> Values)> GetContentByCssSelectorAsync(string html, string nameSelector,
        string genresSelector);
}