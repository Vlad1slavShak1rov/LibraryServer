using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.DTO.Tests;
using LibraryServer.Model;
using System.Text;
using System.Text.Json;

namespace LibraryServer.Service
{
    public class OpenRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string OpenRouterApiUrl = "https://openrouter.ai/api/v1/chat/completions";
        public OpenRouteService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenRouteApi"];

            _httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }

            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "School Test Generator");
        }

        public async Task<GeneratedTestDTO> GenerateTestAsync(int bookId, int questQuantity, string bookTitle)
        {
            try
            {
                if (questQuantity <= 0) throw new Exception($"{nameof(questQuantity)} can not be equal zero!");
                if (questQuantity > 10) throw new Exception($"AI limitation\nThere should be no more than 10 questions");
                var requestBody = new
                {
                    model = "openrouter/free",

                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = $@"Ты - опытный филолог. Генерируешь тесты по литературе и русскому языку.

                            ТВОЯ ЗАДАЧА:
                            Создай тест из {questQuantity} вопросов. Каждый вопрос должен проверять знание темы.

                            ТРЕБОВАНИЯ К ФОРМАТУ:
                            Верни ТОЛЬКО JSON (без лишнего текста) в такой структуре:
                    
                            {{
                              ""subject"": ""Литература"",
                              ""questions"": [
                                {{
                                  ""number"": 1,
                                  ""text"": ""Текст вопроса"",
                                  ""options"": [
                                    ""Первый вариант"",
                                    ""Второй вариант"", 
                                    ""Третий вариант"",
                                    ""Четвертый вариант""
                                  ],
                                  ""correctAnswer"": 0,
                                  ""explanation"": ""Почему этот ответ правильный""
                                }}
                              ]
                            }}

                            ПРАВИЛА:
                            - correctAnswer: 0 = первый вариант, 1 = второй, 2 = третий, 3 = четвертый
                            - Вопросы должны быть разной сложности
                            - Неправильные варианты должны быть правдоподобными
                            - explanation должно объяснять, почему ответ верный"
                        },
                        new
                        {
                            role = "user",
                            content = $"Создай тест по теме: {bookTitle} для 5 класса"
                        }
                    },
                    temperature = 0.3,
                    max_tokens = 8000
                };


                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(OpenRouterApiUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"OpenRouter API error: {error}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(jsonResponse);
                var content = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();


                var cleanedContent = CleanJsonResponse(content);


                var test = JsonSerializer.Deserialize<GeneratedTestDTO>(cleanedContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return test ?? new GeneratedTestDTO
                {
                    Subject = bookTitle,
                    Questions = new List<GeneratedQuestionDTO>()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CleanJsonResponse(string response)
        {
           
            response = response.Trim();

            if (response.StartsWith("```json"))
            {
                response = response.Substring(7); 
                response = response.Substring(0, response.LastIndexOf("```"));
            }
          
            else if (response.StartsWith("```"))
            {
                response = response.Substring(3);
                response = response.Substring(0, response.LastIndexOf("```"));
            }

            return response.Trim();
        }
    }
}