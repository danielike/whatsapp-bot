namespace WhatsappBot.Command;

using Microsoft.Extensions.Options;
using Options;
using RandomContentGenerator;

public class CommandService : ICommandService
{
    private const string Random = "random";
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;
    private readonly IRandomContentQueue _randomContentQueue;

    public CommandService(IOptionsMonitor<ConfigurationOptions> configuration, IRandomContentQueue randomContentQueue)
    {
        _configuration = configuration;
        _randomContentQueue = randomContentQueue;
    }
    
    public void TriggerCommandFunction(CommandResult command)
    {
        switch (command.Command)
        {
            case Random:
                // Example: /random amount @username
                if (_configuration.CurrentValue.GenerateRandomContentEnabled && CommandParser.TryGetIntArg(command, 0, out int amount) &&
                    CommandParser.TryGetMention(command, 1, out string mention))
                {
                    _randomContentQueue.Enqueue(new BackgroundWorkItem(amount, mention, string.Empty));
                }
                break;
        }
    }
}