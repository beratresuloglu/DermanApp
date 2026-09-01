namespace Derman.Web.Models;

public record CreateHelpRequestDto(string Category, string Description, decimal Latitude, decimal Longitude);

public record HelpRequestResponseDto(
    Guid Id, string Category, string Description,
    string? SuggestedUrgency, string? UrgencyReasoning,
    string Status, decimal Latitude, decimal Longitude, DateTime CreatedAt);