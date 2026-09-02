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
    public string[] ForbiddenGenres { get; init; } = [];
    public string FlaresolverrUrl { get; init; } = string.Empty;
    public string SiteUrl { get; init; } = string.Empty;
    public string NameSelector { get; init; } = string.Empty;
    public string GenresSelector { get; init; } = string.Empty;
    public string EvolutionApiUrl { get; init; } = string.Empty;
    public string EvolutionApiSendMessageEndpoint { get; init; } = string.Empty;
    public string EvolutionApiInstance { get; init; } = string.Empty;
    public string EvolutionApiKey { get; init; } = string.Empty;
    public bool GenerateRandomContentEnabled { get; init; }
    public bool TranscribeAudioEnabled { get; init; }
    public string EvolutionApiTranscriberUrl { get; init; } = string.Empty;
    public string EvolutionApiTranscriberEndpoint { get; init; } = string.Empty;
    public string EvolutionApiTranscriberModel { get; init; } = string.Empty;
    public string[] AllowedJids { get; init; } = [];
}

