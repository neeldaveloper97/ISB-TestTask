using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PropertyInventory.Application.Interfaces;

namespace PropertyInventory.Infrastructure.Services;

/// <summary>
/// Converts amounts to USD using historical rates from frankfurter.app.
/// Rates are cached in memory keyed by (currency, date) so the same lookup never hits the API twice.
/// Any failure results in null so the caller can render a fallback.
/// </summary>
public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(HttpClient httpClient, IMemoryCache cache, ILogger<CurrencyService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<decimal?> ConvertToUsdAsync(decimal amount, string fromCurrency, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(fromCurrency))
            return null;

        var currency = fromCurrency.Trim().ToUpperInvariant();

        // Already USD — pass through, no API call.
        if (currency == "USD")
            return amount;

        var rate = await GetUsdRateAsync(currency, date);
        if (rate is null)
            return null;

        return Math.Round(amount * rate.Value, 2);
    }

    private async Task<decimal?> GetUsdRateAsync(string currency, DateTime date)
    {
        var dateKey = date.ToString("yyyy-MM-dd");
        var cacheKey = $"fx:{currency}:{dateKey}:USD";

        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
            return cachedRate;

        try
        {
            var url = $"https://api.frankfurter.app/{dateKey}?from={currency}&to=USD";
            using var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Frankfurter returned {Status} for {Url}", response.StatusCode, url);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var payload = await JsonSerializer.DeserializeAsync<FrankfurterResponse>(stream);

            if (payload?.Rates is not null && payload.Rates.TryGetValue("USD", out var rate))
            {
                _cache.Set(cacheKey, rate, TimeSpan.FromHours(12));
                return rate;
            }

            _logger.LogWarning("Frankfurter response missing USD rate for {Currency} on {Date}", currency, dateKey);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Currency conversion failed for {Currency} on {Date}", currency, dateKey);
            return null;
        }
    }

    private sealed class FrankfurterResponse
    {
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("base")] public string? Base { get; set; }
        [JsonPropertyName("date")] public string? Date { get; set; }
        [JsonPropertyName("rates")] public Dictionary<string, decimal>? Rates { get; set; }
    }
}
