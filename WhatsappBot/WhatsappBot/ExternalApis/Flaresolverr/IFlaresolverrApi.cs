namespace WhatsappBot.ExternalApis.Flaresolverr;

public interface IFlaresolverrApi
{
    Task<string> GetHtml(string flareSolverrUrl, string siteUrl);
}