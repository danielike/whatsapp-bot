namespace WhatsappBotTests;

using WhatsappBot.ExternalApis.Evolution;

public class EvolutionDelayCalculatorTest
{
    [Fact]
    public void ShouldReturnDelay()
    {
        var evolutionDelayCalculator = new EvolutionDelayCalculator();
        
        var result = evolutionDelayCalculator.GetHumanDelay("test message");
        
        Assert.IsType<int>(result);
    }
    
    [Fact]
    public void ShouldReturnDefaultDelay()
    {
        var evolutionDelayCalculator = new EvolutionDelayCalculator();
        
        var result = evolutionDelayCalculator.GetHumanDelay(null!);

        Assert.Equal(EvolutionDelayCalculator.DefaultSafeDelay, result);
    }
}