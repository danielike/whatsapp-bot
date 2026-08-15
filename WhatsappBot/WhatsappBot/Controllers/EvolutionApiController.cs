namespace WhatsappBot.Controllers;

using Microsoft.Extensions.Options;
using MessageWorker;
using Options;
using System.Net.Mime;
using System.Text.Json;
using Command;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EvolutionApiController : ControllerBase
{
    private readonly ILogger<EvolutionApiController> _logger;
    private readonly ICommandService _commandService;
    private readonly IMessageQueue _messageQueue;
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;

    public EvolutionApiController(ILogger<EvolutionApiController> logger, ICommandService commandService, IMessageQueue messageQueue, IOptionsMonitor<ConfigurationOptions> configuration)
    {
        _logger = logger;
        _commandService = commandService;
        _messageQueue = messageQueue;
        _configuration = configuration;
    }

    [HttpPost("messages")]
    [Consumes(typeof(EvolutionPayload), MediaTypeNames.Application.Json)]
    public IActionResult GetSendMessageEvent([FromBody] object payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload.ToString()!);
            var json = JsonSerializer.Deserialize<EvolutionPayload>(doc.RootElement.GetRawText());

            if (json!.eventName == EvolutionPayload.MessagesUpsert && json.data.messageType != EvolutionPayload.AudioMessage)
            {
                var command = CommandParser.Parse(json.data.message.conversation);

                if (command is not null)
                {
                    _commandService.TriggerCommandFunction(command);
                }
            }
            
            if (_configuration.CurrentValue.TranscribeAudioEnabled && json is { eventName: EvolutionPayload.MessagesUpsert, data.messageType: EvolutionPayload.AudioMessage })
            {
                _messageQueue.Enqueue(new BackgroundWorkItem(0, json.data.key.participant, json.data.message.base64));
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