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
    public string Id { get; }
    public int Interval { get; }
    public BackgroundMessageType MessageType { get; }

    public BackgroundWorkItem(int amount, string mention, string base64Audio, string id, int interval = 0, BackgroundMessageType messageType = BackgroundMessageType.None)
    {
        Amount = amount;
        Mention = mention;
        Base64Audio = base64Audio;
        Id = id;
        Interval = interval;
        MessageType = messageType;
    }

    public enum BackgroundMessageType
    {
        None = 0,
        RandomCommand = 1,
        BuzzCommand = 2,
        StopCommand = 3,
        AudioTranscription = 4
    }
}