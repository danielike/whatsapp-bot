using System.Net;
using System.Text.Json;
using WhatsappBot.Command;
using Microsoft.AspNetCore.Mvc;

namespace WhatsappBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvolutionApiController : ControllerBase
{
    private readonly ILogger<EvolutionApiController> _logger;
    private readonly ICommandService _commandService;

    public EvolutionApiController(ILogger<EvolutionApiController> logger, ICommandService commandService)
    {
        _logger = logger;
        _commandService = commandService;
    }

    [HttpPost("messages")]
    public async Task<IActionResult> GetSendMessageEvent([FromBody] object payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload.ToString()!);
            var json = JsonSerializer.Deserialize<EvolutionPayload>(doc.RootElement.GetRawText());

            if (json!.eventName == EvolutionPayload.MessagesUpsert && json!.data.messageType != EvolutionPayload.AudioMessage)
            {
                var command = CommandParser.Parse(json.data.message.conversation);

                if (command is not null)
                {
                    await _commandService.TriggerCommandFunction(command);
                }
            }
            // TODO: Implement audio transcription
            if (json!.eventName == EvolutionPayload.MessagesUpsert &&
                json!.data.messageType == EvolutionPayload.AudioMessage)
            {
                throw new NotImplementedException();
            }
        }
        catch (Exception e)
        {
            _logger.NotValidJson(e);
            // We don't return any status error to avoid retries, because it's an unexpected, unnecessary json in our logic.
        }

        return Ok();
    }
}