namespace Derman.Web.Models;

public record CreateReportDto(Guid ReportedUserId, string Reason);