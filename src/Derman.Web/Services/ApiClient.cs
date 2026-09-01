using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Derman.Web.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TokenService _tokenService;

    public ApiClient(IHttpClientFactory httpClientFactory, TokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
    }

    private async Task<HttpClient> CreateAuthorizedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("DermanApi");
        var token = await _tokenService.GetTokenAsync();

        Console.WriteLine($"[API DEBUG] Token var mı: {!string.IsNullOrWhiteSpace(token)}, uzunluk: {token?.Length ?? 0}");
        if (!string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine($"[API DEBUG] Token başlangıcı: {token.Substring(0, Math.Min(20, token.Length))}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"[API DEBUG] Header ayarlandı: {client.DefaultRequestHeaders.Authorization}");
        }

        return client;
    }
    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        var client = await CreateAuthorizedClientAsync();
        return await client.PostAsJsonAsync(url, body);
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var client = await CreateAuthorizedClientAsync();
        return await client.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T body)
    {
        var client = await CreateAuthorizedClientAsync();
        return await client.PutAsJsonAsync(url, body);
    }
}