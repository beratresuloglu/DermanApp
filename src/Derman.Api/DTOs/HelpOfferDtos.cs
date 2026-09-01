namespace Derman.Api.DTOs;

public record CreateHelpOfferDto(string Category, int Quantity, decimal Latitude, decimal Longitude);

public record HelpOfferResponseDto(
    Guid Id, string Category, int Quantity, string Status,
    decimal Latitude, decimal Longitude, DateTime CreatedAt);