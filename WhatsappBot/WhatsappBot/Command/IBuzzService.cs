namespace WhatsappBot.Command;

public interface IBuzzService
{
    Guid StartBuzz(TimeSpan interval, string id, string mention);
    void StopAll();
    void Stop(Guid id);
}