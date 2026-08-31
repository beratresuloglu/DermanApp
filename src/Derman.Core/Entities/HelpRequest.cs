using Derman.Core.Enums;

namespace Derman.Core.Entities;

public class HelpRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UrgencyLevel? SuggestedUrgency { get; set; }
    public string? UrgencyReasoning { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Acik;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}