using Derman.Core.Enums;

namespace Derman.Core.Entities;

public class HelpOffer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Acik;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}