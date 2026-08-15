namespace WhatsappBot.Command;

using Microsoft.Extensions.Options;
using Options;
using MessageWorker;

public class CommandService : ICommandService
{
    private const string Random = "random";
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;
    private readonly IMessageQueue _messageQueue;

    public CommandService(IOptionsMonitor<ConfigurationOptions> configuration, IMessageQueue messageQueue)
    {
        _configuration = configuration;
        _messageQueue = messageQueue;
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
                    _messageQueue.Enqueue(new BackgroundWorkItem(amount, mention, string.Empty));
                }
                break;
        }
    }
}