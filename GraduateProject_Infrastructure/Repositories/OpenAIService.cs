using GraduateProject_Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["OpenAI:ApiKey"]}");
        }

        public async Task<string> GetDiagnosisFromAI(string symptoms)
        {
            return await Task.FromResult("Possible Diagnosis: Viral Infection");
        }

        public async Task<string> GetChatbotResponseAsync(string question)
        {
            int maxRetries = 3;
            int retryDelayMs = 1000;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var patientContext = "You are a medical assistant designed to help patients. Provide clear, simple, and safe answers tailored for a patient. Use easy-to-understand language, avoid complex medical terms unless necessary, and always recommend consulting a doctor for serious concerns. Do not provide definitive diagnoses, only general advice or information.";

                    var requestBody = new
                    {
                        model = _config["OpenAI:Model"] ?? "gpt-3.5-turbo",
                        messages = new[]
                        {
                            new { role = "system", content = patientContext },
                            new { role = "user", content = question }
                        },
                        max_tokens = 150,
                        temperature = 0.7
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync("chat/completions", content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseBody);
                    var answer = jsonDoc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return answer?.Trim() ?? "Sorry, I couldn't generate a response. Please try again.";
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("429"))
                {
                    if (attempt == maxRetries - 1)
                        return "Error: Too many requests. Please check your API usage limits.";
                    await Task.Delay(retryDelayMs);
                    retryDelayMs *= 2;
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("403"))
                {
                    return "Error: Access forbidden. Please check your OpenAI API key.";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }

            return "Error: Failed to get a response after multiple attempts.";
        }
    }
}