using Derman.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Derman.Api.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string? TcKimlikNoHash { get; set; }
    public bool IsBlocked { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}