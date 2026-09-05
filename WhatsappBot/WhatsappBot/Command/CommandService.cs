namespace WhatsappBot.Command;

using Microsoft.Extensions.Options;
using Options;
using MessageWorker;

public class CommandService : ICommandService
{
    private const string Random = "random";
    private const string Buzz = "buzz";
    private const string Stop = "stop";
    private readonly IOptionsMonitor<ConfigurationOptions> _configuration;
    private readonly IMessageQueue _messageQueue;
    private readonly IBuzzService _buzzService;

    public CommandService(IOptionsMonitor<ConfigurationOptions> configuration, IMessageQueue messageQueue, IBuzzService buzzService)
    {
        _configuration = configuration;
        _messageQueue = messageQueue;
        _buzzService = buzzService;
    }
    
    public void TriggerCommandFunction(CommandResult command, string id)
    {
        switch (command.Command)
        {
            case Random:
                // Example: /random amount @username
                if (_configuration.CurrentValue.GenerateRandomContentEnabled && CommandParser.TryGetIntArg(command, 0, out int amount) &&
                    CommandParser.TryGetMention(command, 1, out string mention))
                {
                    _messageQueue.Enqueue(new BackgroundWorkItem(amount, mention, string.Empty, id, 0, BackgroundWorkItem.BackgroundMessageType.RandomCommand));
                }
                break;
            case Buzz:
                if (_configuration.CurrentValue.BuzzCommandEnabled &&
                    CommandParser.TryGetIntArg(command, 0, out int interval) &&
                    CommandParser.TryGetMention(command, 1, out string buzzMention))
                {
                    _messageQueue.Enqueue(new BackgroundWorkItem(0, buzzMention, string.Empty, id, interval, BackgroundWorkItem.BackgroundMessageType.BuzzCommand));
                }
                break;
            case Stop:
                if (_configuration.CurrentValue.BuzzCommandEnabled)
                {
                    _buzzService.StopAll();
                }
                break;
        }
    }
}