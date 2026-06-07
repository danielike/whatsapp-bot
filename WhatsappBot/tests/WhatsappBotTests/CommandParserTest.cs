using WhatsappBot;

namespace WhatsappBotTests;

public class CommandParserTest
{
    private const string Command = "/test"; 
    private const string NoCommand = "test";
    private const string Argument = "argument";
    private const string Argument2 = "argument2";
    
    [Fact]
    public void ShouldParseCommand()
    {
        CommandResult? result = CommandParser.Parse($"{Command}");
        Assert.NotNull(result);
    }

    [Fact]
    public void ShouldNotParseCommand()
    {
        CommandResult? result = CommandParser.Parse($"{NoCommand}");
        Assert.Null(result);
    }

    [Fact]
    public void ShouldParseCommandWithArgument()
    {
        CommandResult? result = CommandParser.Parse($"{Command} {Argument}");
        Assert.NotNull(result);
        Assert.Contains(result.Args, arg => arg == Argument);
    }
    
    [Fact]
    public void ShouldParseCommandWithArguments()
    {
        CommandResult? result = CommandParser.Parse($"{Command} {Argument} {Argument2}");
        Assert.NotNull(result);
        Assert.Contains(result.Args, arg => arg == Argument);
        Assert.Contains(result.Args, arg => arg == Argument2);
    }
}
