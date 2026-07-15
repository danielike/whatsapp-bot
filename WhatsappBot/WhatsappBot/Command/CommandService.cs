using Microsoft.Extensions.Options;
using WhatsappBot.ExternalApis;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.Options;

namespace WhatsappBot.Command;

public class CommandService : ICommandService
{
    private const string Random = "random";
    private readonly IEvolutionApiService _evolutionApiService;

    public CommandService(IEvolutionApiService evolutionApiService)
    {
        _evolutionApiService = evolutionApiService;
    }
    public async Task<HttpResponseMessage> TriggerCommandFunction(CommandResult command)
    {
        switch (command.Command)
        {
            case Random:
                // Example: /random amount @username
                if (CommandParser.TryGetIntArg(command, 0, out int amount) &&
                    CommandParser.TryGetMention(command, 1, out string mention))
                {
                    // TODO: Get based on number from website, and send message
                    return await _evolutionApiService.SendMessage($"{mention}");;
                }
                break;
        }

        return new HttpResponseMessage();
    }
}