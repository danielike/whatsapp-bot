using WhatsappBot.ExternalApis.Evolution;

namespace WhatsappBotTests;

public class EvolutionDelayCalculatorTest
{
    [Fact]
    public void ShouldReturnDelay()
    {
        var result = EvolutionDelayCalculator.GetHumanDelay("test message");

        Assert.IsType<int>(result);
    }
    
    [Fact]
    public void ShouldReturnDefaultDelay()
    {
        var result = EvolutionDelayCalculator.GetHumanDelay(null!);

        Assert.Equal(EvolutionDelayCalculator.DefaultSafeDelay, result);
    }
}