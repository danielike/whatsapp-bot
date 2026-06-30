using System.Text.Json;

namespace WhatsappBot.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EvolutionApiController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Ok();
    }
    
    [HttpPost("sendMessage")]
    public IActionResult GetSendMessageEvent([FromBody] object payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload.ToString()!);
            var json = JsonSerializer.Deserialize<EvolutionPayload>(doc.RootElement.GetRawText());

            if (json!.eventName == EvolutionPayload.MessagesUpsert)
            {
                var command = CommandParser.Parse(json.data.message.conversation);

                if (command is not null)
                {
                    // Example: /CommandName 1 @username
                    CommandParser.TryGetIntArg(command, 0, out int amount);
                    CommandParser.TryGetMention(command, 1, out string mention);
                }
            }
        }
        catch
        {
            Console.WriteLine("Body was not valid JSON.");
        }
        return Ok();
    }
}