namespace WhatsappBot.RandomContentGenerator;

using Microsoft.Extensions.Options;
using DomManipulator;
using ExternalApis.Flaresolverr;
using Options;

public class RandomContentGenerator : IRandomContentGenerator
{
    private readonly ILogger<RandomContentGenerator> _logger;
    private readonly IOptionsMonitor<ConfigurationOptions>  _configuration;
    private readonly IFlaresolverrApi _flaresolverrApi;
    private readonly IDomManipulator _domManipulator;

    public RandomContentGenerator(
        ILogger<RandomContentGenerator> logger,
        IOptionsMonitor<ConfigurationOptions> configuration,
        IFlaresolverrApi flaresolverrApi,
        IDomManipulator domManipulator)
    {
        _logger = logger;
        _configuration = configuration;
        _flaresolverrApi = flaresolverrApi;
        _domManipulator = domManipulator;
    }
    
    public async Task<string> Generate(int number, string mention)
    {
        HashSet<string> forbiddenGenres = new( 
            _configuration.CurrentValue.ForbiddenGenres
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s)),
            StringComparer.OrdinalIgnoreCase
        );
        
        _logger.LogGeneratorRunningAt(DateTimeOffset.Now);
        
        var contents = new (string? Key, IEnumerable<string?> Values)[number];
        try
        {
            for(int i = 0; i < number; i++)
            {
                var result = await _domManipulator.GetContentByCssSelectorAsync(
                    await _flaresolverrApi.GetHtml(_configuration.CurrentValue.FlaresolverrUrl,
                        _configuration.CurrentValue.SiteUrl), _configuration.CurrentValue.NameSelector,
                    _configuration.CurrentValue.GenresSelector, forbiddenGenres);
    
                // repeat the get html content until any series without forbidden genres
                while (result.Key!.Length == 0 && !result.Values.Any())
                {
                    result = await _domManipulator.GetContentByCssSelectorAsync(
                        await _flaresolverrApi.GetHtml(_configuration.CurrentValue.FlaresolverrUrl,
                            _configuration.CurrentValue.SiteUrl), _configuration.CurrentValue.NameSelector,
                        _configuration.CurrentValue.GenresSelector, forbiddenGenres);
                }
                
                contents[i] = result;
            }
        }
        catch(Exception e)
        {
            _logger.ErrorGettingSelector(e);
        }
    
        var seriesNames = string.Join('\n', contents.Select(seriesName => seriesName.Key));
        
        _logger.LogContentGenerated($"{seriesNames} => ");
        _logger.LogContentGenerated($"{ string.Join(",\n", contents.SelectMany(genres => genres.Values).Select(genre => genre ?? string.Empty ) ) }");
        
        return $"{mention}\n{seriesNames}";
    }
}