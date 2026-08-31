using Derman.Core.Enums;

namespace Derman.Api.DTOs;

public record RegisterRequest(string FullName, string Email, string Password, Role Role);
public record VerifyOtpRequest(string Email, string Code);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string FullName, Role Role);