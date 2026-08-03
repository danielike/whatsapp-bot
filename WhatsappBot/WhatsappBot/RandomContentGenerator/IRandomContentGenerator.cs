namespace WhatsappBot.RandomContentGenerator;

public interface IRandomContentGenerator
{
    Task<string> Generate(int number, string mention, CancellationToken token = default);
}