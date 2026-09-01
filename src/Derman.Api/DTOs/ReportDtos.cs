namespace Derman.Api.DTOs;

public record CreateReportDto(Guid ReportedUserId, string Reason);

public record ReportResponseDto(Guid Id, Guid ReporterId, Guid ReportedUserId, string Reason, string Status, DateTime CreatedAt);