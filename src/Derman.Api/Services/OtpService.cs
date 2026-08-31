using System.Collections.Concurrent;

namespace Derman.Api.Services;

public class OtpService
{
    private readonly ConcurrentDictionary<string, string> _codes = new();

    public string GenerateCode(string email)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        _codes[email] = code;

        // Gerçek e-posta servisi bağlanana kadar, kodu konsola yazdırıyoruz
        Console.WriteLine($"[OTP] {email} için kod: {code}");

        return code;
    }

    public bool ValidateCode(string email, string code)
    {
        if (_codes.TryGetValue(email, out var storedCode) && storedCode == code)
        {
            _codes.TryRemove(email, out _);
            return true;
        }
        return false;
    }
}