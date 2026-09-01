using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Derman.Web.Services;

public class TokenService
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private const string TokenKey = "auth_token";

    public TokenService(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task SetTokenAsync(string token)
    {
        await _sessionStorage.SetAsync(TokenKey, token);
        Console.WriteLine($"[TOKEN DEBUG] Token kaydedildi, uzunluk: {token.Length}");
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            Console.WriteLine($"[TOKEN DEBUG] Token bulundu mu: {result.Success}, Değer var mı: {!string.IsNullOrEmpty(result.Value)}");
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TOKEN DEBUG] Hata: {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }

    public async Task ClearTokenAsync()
    {
        await _sessionStorage.DeleteAsync(TokenKey);
    }
}