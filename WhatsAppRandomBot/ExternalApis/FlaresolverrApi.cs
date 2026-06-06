namespace WhatsAppRandomBot.ExternalApis;

using System.Text;
using System.Text.Json;

public static class FlaresolverrApi
{
    public static async Task<string> GetHtml(HttpClient client, string flareSolverrUrl, string siteUrl)
    {
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
}