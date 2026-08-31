using Derman.Core.Enums;

namespace Derman.Core.Entities;

public class Match
{
    public Guid Id { get; set; }
    public Guid HelpRequestId { get; set; }
    public Guid HelperUserId { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Bekliyor;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}