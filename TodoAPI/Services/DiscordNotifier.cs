using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoAPI.Services
{
    public static class DiscordNotifier
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string WebhookUrl = "https://discord.com/api/webhooks/1369423486258511895/4YtO63hdZo8nxoMH_BnoBMqY09uHxu1vO4Lv3MfeVHO-M-yeevlovByYhqPgfSXvwl0R";

        public static async Task SendNotification(string message)
        {
            var payload = new
            {
                content = message
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(WebhookUrl, content);
        }
    }
}