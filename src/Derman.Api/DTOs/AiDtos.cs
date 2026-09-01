namespace Derman.Api.DTOs;

// Gemini API'ye giden istek gövdesi
public class GeminiRequest
{
    public GeminiContent[] Contents { get; set; } = [];
}

public class GeminiContent
{
    public GeminiPart[] Parts { get; set; } = [];
}

public class GeminiPart
{
    public string Text { get; set; } = string.Empty;
}

// Gemini API'den dönen yanıt (sadece ihtiyacımız olan kısımlar)
public class GeminiResponse
{
    public GeminiCandidate[]? Candidates { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent? Content { get; set; }
}

// Modelin ürettiği JSON'ı bu şekle çözeceğiz
public record TriageResult(string Urgency, string Reasoning);

public record RegionPriorityResult(string Summary, List<string> PriorityRequestIds);