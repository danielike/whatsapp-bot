using System.Text.RegularExpressions;

namespace WhatsappBot;

public record CommandResult(string Command, List<string> Args);

public static partial class CommandParser
{
    [GeneratedRegex(@"^/([^\s]+)\s*(.*)$")]
    private static partial Regex CommandRegex();
    private static Match CommandMatch(string input) => CommandRegex().Match(input);

    [GeneratedRegex(@"""([^""]+)""|'([^']+)'|(\S+)")]
    private static partial Regex ArgumentsRegex();
    private static MatchCollection ArgumentsMatches(string input) => ArgumentsRegex().Matches(input);
    
    // Returns null if input is not a command (doesn't start with '/')
    public static CommandResult? Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        text = text.Trim();

        var commandMatch = CommandMatch(text);
        if (!commandMatch.Success) return null;

        var command = commandMatch.Groups[1].Value;
        var rest = commandMatch.Groups[2].Value;

        // split rest into args, supporting double-quoted and single-quoted segments
        var args = new List<string>();
        foreach (Match argumentsMatch in ArgumentsMatches(rest))
        {
            var val = argumentsMatch.Groups[1].Success ? argumentsMatch.Groups[1].Value
                : argumentsMatch.Groups[2].Success ? argumentsMatch.Groups[2].Value
                : argumentsMatch.Groups[3].Value;
            args.Add(val);
        }

        return new CommandResult(command, args);
    }

    // Helper: try parse first arg as int
    public static bool TryGetIntArg(CommandResult parsed, int index, out int value)
    {
        value = 0;
        if (parsed.Args.Count <= index) return false;
        return int.TryParse(parsed.Args[index], out value);
    }

    // Helper: get mention arg starting with '@'
    public static bool TryGetMention(CommandResult parsed, int index, out string mention)
    {
        mention = string.Empty;
        if (parsed.Args.Count <= index) return false;
        var candidate = parsed.Args[index];
        if (!candidate.StartsWith('@')) return false;
        mention = candidate;
        return true;
    }
}