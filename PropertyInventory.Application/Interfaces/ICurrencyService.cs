namespace PropertyInventory.Application.Interfaces;

public interface ICurrencyService
{
    /// <summary>
    /// Converts <paramref name="amount"/> from <paramref name="fromCurrency"/> to USD using the
    /// historical rate on <paramref name="date"/>. Returns null if the conversion could not be performed.
    /// </summary>
    Task<decimal?> ConvertToUsdAsync(decimal amount, string fromCurrency, DateTime date);
}
