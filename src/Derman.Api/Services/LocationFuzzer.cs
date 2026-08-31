namespace Derman.Api.Services;

public static class LocationFuzzer
{
    // Koordinatı ~2 ondalık basamağa yuvarlar, bu yaklaşık 1.1 km'lik bir belirsizlik alanı yaratır
    public static (decimal Lat, decimal Lng) Fuzz(decimal lat, decimal lng)
    {
        return (Math.Round(lat, 2), Math.Round(lng, 2));
    }
}