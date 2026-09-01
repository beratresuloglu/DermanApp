namespace Derman.Web.Models;

public record RegisterRequest(string FullName, string Email, string Password, string Role);
public record VerifyOtpRequest(string Email, string Code);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string FullName, string Role);