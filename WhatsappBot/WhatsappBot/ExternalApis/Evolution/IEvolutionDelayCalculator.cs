namespace WhatsappBot.ExternalApis.Evolution;

public interface IEvolutionDelayCalculator
{
    int GetHumanDelay(string message, int wpm = 50);
}