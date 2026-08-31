using Derman.Core.Enums;

namespace Derman.Api.Services;

public interface IAiTriageService
{
    Task<(UrgencyLevel Urgency, string Reasoning)> ScoreUrgencyAsync(string description);
}