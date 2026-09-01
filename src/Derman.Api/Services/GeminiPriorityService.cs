using System.Text;
using System.Text.Json;
using Derman.Api.DTOs;
using Derman.Core.Entities;

namespace Derman.Api.Services;

public class GeminiPriorityService : IAiPriorityService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GeminiPriorityService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<(string Summary, List<Guid> PriorityIds)> AnalyzeRegionAsync(List<HelpRequest> nearbyRequests)
    {
        if (nearbyRequests.Count == 0)
            return ("Bu bölgede şu anda açık talep bulunmuyor.", new List<Guid>());

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var apiKey = _config["AI:GeminiApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        var requestList = string.Join("\n", nearbyRequests.Select(r =>
            $"- Id: {r.Id}, Kategori: {r.Category}, Açıklama: \"{r.Description}\", Aciliyet: {r.SuggestedUrgency}"));

        var prompt = $$"""
            Aşağıda bir bölgedeki açık afet yardım talepleri listelenmiştir. Bunları önceliklendir.

            {{requestList}}

            SADECE aşağıdaki JSON formatında yanıt ver, başka hiçbir metin ekleme:
            {"summary": "bölge durumu hakkında 2-3 cümlelik kısa özet ve öneri", "priorityRequestIds": ["en öncelikli 1-3 talebin Id'si, önem sırasına göre"]}
            """;

        var requestBody = new GeminiRequest
        {
            Contents = [new GeminiContent { Parts = [new GeminiPart { Text = prompt }] }]
        };

        try
        {
            var json = JsonSerializer.Serialize(requestBody, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson, jsonOptions);

            var rawText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            if (string.IsNullOrWhiteSpace(rawText))
                return ("AI önerisi şu anda alınamadı.", new List<Guid>());

            var cleaned = rawText.Replace("```json", "").Replace("```", "").Trim();
            var result = JsonSerializer.Deserialize<RegionPriorityResult>(cleaned,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
                return ("AI önerisi ayrıştırılamadı.", new List<Guid>());

            var priorityIds = result.PriorityRequestIds
                .Where(id => Guid.TryParse(id, out _))
                .Select(Guid.Parse)
                .ToList();

            return (result.Summary, priorityIds);
        }
        catch (Exception)
        {
            return ("AI servisi şu anda yanıt veremedi.", new List<Guid>());
        }
    }
}