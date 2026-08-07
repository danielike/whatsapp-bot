using Microsoft.Extensions.Options;
using WhatsappBot.ExternalApis;
using WhatsappBot.ExternalApis.Evolution;
using WhatsappBot.Options;
using WhatsappBot.RandomContentGenerator;

namespace WhatsappBot.Command;

public class CommandService : ICommandService
{
    private const string Random = "random";
    private readonly IEvolutionApiService _evolutionApiService;
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;
    private readonly IRandomContentGenerator _randomContentGenerator;

    public CommandService(IEvolutionApiService evolutionApiService, IOptionsMonitor<ConfigurationOptions> configuration, IRandomContentGenerator randomContentGenerator)
    {
        _evolutionApiService = evolutionApiService;
        _configuration = configuration;
        _randomContentGenerator = randomContentGenerator;
    }
    public async Task<HttpResponseMessage> TriggerCommandFunction(CommandResult command)
    {
        switch (command.Command)
        {
            case Random:
                // Example: /random amount @username
                if (_configuration.CurrentValue.GenerateRandomContentEnabled && CommandParser.TryGetIntArg(command, 0, out int amount) &&
                    CommandParser.TryGetMention(command, 1, out string mention))
                {
                    return await _evolutionApiService.SendMessage(_configuration.CurrentValue.EvolutionApiSendMessageId, await _randomContentGenerator.Generate(amount, mention));
                }
                break;
        }

        return new HttpResponseMessage();
    }
}