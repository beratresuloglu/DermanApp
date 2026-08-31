using Derman.Core.Enums;

namespace Derman.Core.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public Guid ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Beklemede;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}