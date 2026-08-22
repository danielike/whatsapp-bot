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
    public BackgroundMessageType MessageType { get; }

    public BackgroundWorkItem(int amount, string mention, string base64Audio, string id, BackgroundMessageType messageType = BackgroundMessageType.None)
    {
        Amount = amount;
        Mention = mention;
        Base64Audio = base64Audio;
        Id = id;
        MessageType = messageType;
    }

    public enum BackgroundMessageType
    {
        None = 0,
        Command = 1,
        AudioTranscription = 2
    }
}