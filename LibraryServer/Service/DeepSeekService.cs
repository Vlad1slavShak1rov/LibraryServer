using System.Text;
using System.Text.Json;

namespace LibraryServer.Service
{
    public class DeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string DeepSeekApiUrl = "https://api.deepseek.com/v1/chat/completions";

        public DeepSeekService(IConfiguration configuration)
        {
            _apiKey = configuration["DeepSeek"];

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetChatResponseAsync(string userPrompt)
        {
            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                new { role = "system", content =  @"Ты - опытный учитель математики. 
                           Генерируй тесты для 9 класса.
                           В каждом вопросе должно быть 4 варианта ответа.
                           Обязательно указывай правильный ответ.
                           Формат ответа строго JSON." },
                new { role = "user", content = $"Создай тест на тему {userPrompt}" }
            },
                temperature = 0.7
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(DeepSeekApiUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
    }
}
