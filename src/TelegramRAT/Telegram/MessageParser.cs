namespace TelegramRAT.Telegram;

using TelegramRAT.Models;

internal static class MessageParser
{
    public static BotCommand? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Trim();

        if (!text.StartsWith('/'))
            return null;

        var parts = text[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return null;

        var command = parts[0].ToLowerInvariant();
        if (command.Contains('@'))
            command = command.Split('@')[0];

        var arguments = parts.Length > 1 ? parts[1..] : [];

        return new BotCommand
        {
            Command = command,
            Arguments = arguments,
            RawText = text
        };
    }
}
