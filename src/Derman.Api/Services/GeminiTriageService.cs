using System.Text;
using System.Text.Json;
using Derman.Api.DTOs;
using Derman.Core.Enums;

namespace Derman.Api.Services;

public class GeminiTriageService : IAiTriageService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GeminiTriageService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<(UrgencyLevel Urgency, string Reasoning)> ScoreUrgencyAsync(string description)
    {
        var apiKey = _config["AI:GeminiApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={apiKey}";

        var prompt = $$"""
            Bir afet yardım talebini analiz et ve aciliyetini belirle.
            Talep metni: "{{description}}"

            SADECE aşağıdaki JSON formatında yanıt ver, başka hiçbir metin ekleme:
            {"urgency": "Dusuk" | "Orta" | "Kritik", "reasoning": "kısa gerekçe (en fazla 20 kelime)"}
            """;

        var requestBody = new GeminiRequest
        {
            Contents = [new GeminiContent { Parts = [new GeminiPart { Text = prompt }] }]
        };

        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(requestBody, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson, jsonOptions);

            var rawText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            if (string.IsNullOrWhiteSpace(rawText))
                return (UrgencyLevel.Orta, "AI yanıtı boş geldi, varsayılan değer atandı.");

            // Modelin bazen JSON'ı ```json ... ``` bloğu içine sarma ihtimaline karşı temizleme
            var cleaned = rawText.Replace("```json", "").Replace("```", "").Trim();

            var result = JsonSerializer.Deserialize<TriageResult>(cleaned,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null || !Enum.TryParse<UrgencyLevel>(result.Urgency, true, out var urgency))
                return (UrgencyLevel.Orta, "AI yanıtı ayrıştırılamadı, varsayılan değer atandı.");

            return (urgency, result.Reasoning);
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"[AI HATA] {ex.GetType().Name}: {ex.Message}");
            return (UrgencyLevel.Orta, "AI servisi şu anda yanıt veremedi, varsayılan değer atandı.");
        }
    }
}