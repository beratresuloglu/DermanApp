namespace Derman.Web.Models;

public record ResourceResponseDto(
    Guid Id, string Name, string Type, string Address, string? Phone,
    decimal Latitude, decimal Longitude);