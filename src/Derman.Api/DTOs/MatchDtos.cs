namespace Derman.Api.DTOs;

public record CreateMatchDto(Guid HelpRequestId);

public record MatchResponseDto(
    Guid Id,
    Guid HelpRequestId,
    Guid HelperUserId,
    string Status,
    DateTime RequestedAt,
    DateTime? ConfirmedAt);