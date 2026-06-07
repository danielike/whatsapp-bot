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
    public string ForbiddenGenres { get; set; }
    public string FlaresolverrUrl { get; set; }
    public string SiteUrl { get; set; }
    public string NameSelector { get; set; }
    public string GenresSelector { get; set; }
}

