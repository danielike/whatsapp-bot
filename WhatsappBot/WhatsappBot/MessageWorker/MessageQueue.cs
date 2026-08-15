namespace WhatsappBot.MessageWorker;

using System.Threading.Channels;

public class MessageQueue : IMessageQueue
{
    private readonly Channel<BackgroundWorkItem> _channel = Channel.CreateUnbounded<BackgroundWorkItem>(
    new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });
    
    public void Enqueue(BackgroundWorkItem item) => _channel.Writer.TryWrite(item);
    
    public ValueTask<BackgroundWorkItem> DequeueAsync(CancellationToken ct) => _channel.Reader.ReadAsync(ct);
}