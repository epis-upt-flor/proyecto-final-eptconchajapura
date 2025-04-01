using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoCF.Services
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _groupId;

        public TelegramService(string botToken, string groupId)
        {
            _botToken = botToken ?? throw new ArgumentNullException(nameof(botToken));
            _groupId = groupId ?? throw new ArgumentNullException(nameof(groupId));
        }

        public async Task SendMessageAsync(string message)
        {
            using (var client = new HttpClient())
            {
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

                var content = new StringContent(
                    $"{{\"chat_id\":\"{_groupId}\",\"text\":\"{message}\"}}",
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al enviar mensaje a Telegram: {error}");
                }
            }
        }
    }
}