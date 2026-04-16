using System.Text;
using System.Text.Json;

namespace DeviceManager.API.Services;

public class AiDescriptionService : IAiDescriptionService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public AiDescriptionService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"]!;
        _model = config["Gemini:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<string> GenerateDescriptionAsync(
        string name, string manufacturer, string type,
        string os, string processor, string ram)
    {
        var prompt = $@"Generate a concise, professional, human-readable description 
(1-2 sentences, max 50 words) for the following device. 
Do not include quotes around the response.

Name: {name}
Manufacturer: {manufacturer}
Type: {type}
Operating System: {os}
Processor: {processor}
RAM: {ram}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Gemini API error: {response.StatusCode} - {responseBody}");
        }

        // Parse the Gemini response
        using var doc = JsonDocument.Parse(responseBody);
        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text?.Trim().Trim('"') ?? "Description could not be generated.";
    }
}