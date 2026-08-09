namespace WhatsappBot.RandomContentGenerator;

public interface IRandomContentQueue
{
    void Enqueue(BackgroundWorkItem item);
    ValueTask<BackgroundWorkItem> DequeueAsync(CancellationToken ct);
}

public sealed class BackgroundWorkItem
{
    public int Amount { get; }
    public string Mention { get; }
    
    public BackgroundWorkItem(int amount, string mention)
    {
        Amount = amount;
        Mention = mention;
    }
}