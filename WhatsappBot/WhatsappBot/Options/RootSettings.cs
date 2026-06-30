namespace WhatsappBot.Options;

public class RootSettings
{
    public Logging Logging { get; set; }
    public ConfigurationOptions ConfigurationOptions { get; set; }
    public string AllowedHosts { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; }
    public string Microsoft_AspNetCore { get; set; }
}

public class ConfigurationOptions
{
    public required string ForbiddenGenres { get; set; }
    public required string FlaresolverrUrl { get; set; }
    public required string SiteUrl { get; set; }
    public required string NameSelector { get; set; }
    public required string GenresSelector { get; set; }
}

