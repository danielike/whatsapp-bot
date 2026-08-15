namespace WhatsappBot.MessageWorker;

public interface IMessageQueue
{
    void Enqueue(BackgroundWorkItem item);
    ValueTask<BackgroundWorkItem> DequeueAsync(CancellationToken ct);
}

public sealed class BackgroundWorkItem
{
    public int Amount { get; }
    public string Mention { get; }
    public string Base64Audio { get; }

    public BackgroundWorkItem(int amount, string mention, string base64Audio)
    {
        Amount = amount;
        Mention = mention;
        Base64Audio = base64Audio;
    }
}