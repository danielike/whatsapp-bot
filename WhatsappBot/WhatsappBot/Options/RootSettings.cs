using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WhatsappBot.Options;

public class RootSettings
{
    public Logging Logging { get; set; } = new  Logging();
    public ConfigurationOptions ConfigurationOptions { get; set; } = new ConfigurationOptions();
    public string AllowedHosts { get; set; } = string.Empty;
}

public class Logging
{
    public LogLevel LogLevel { get; set; } = new LogLevel();
}

public class LogLevel
{
    public string Default { get; set; } = string.Empty;
    public string MicrosoftAspNetCore { get; set; } = string.Empty;
}

public class ConfigurationOptions
{
    public string ForbiddenGenres { get; set; } = string.Empty;
    public string FlaresolverrUrl { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string NameSelector { get; set; } = string.Empty;
    public string GenresSelector { get; set; } = string.Empty;
    public string EvolutionApiUrl { get; set; } = string.Empty;
    public string EvolutionApiEndpoint { get; set; } = string.Empty;
    public string EvolutionApiInstance { get; set; } = string.Empty;
    public string EvolutionApiKey { get; set; } = string.Empty;
}

