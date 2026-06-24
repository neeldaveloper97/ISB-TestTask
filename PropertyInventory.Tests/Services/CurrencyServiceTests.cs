using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using PropertyInventory.Infrastructure.Services;

namespace PropertyInventory.Tests.Services;

public class CurrencyServiceTests
{
    private static CurrencyService Build(HttpMessageHandler handler, MemoryCache? cache = null) =>
        new(new HttpClient(handler), cache ?? new MemoryCache(new MemoryCacheOptions()),
            NullLogger<CurrencyService>.Instance);

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public int CallCount { get; private set; }

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(_responder(request));
        }
    }

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    [Fact]
    public async Task ConvertToUsdAsync_returns_amount_when_currency_is_usd_without_calling_api()
    {
        var handler = new StubHandler(_ => throw new InvalidOperationException("should not be called"));
        var sut = Build(handler);

        var result = await sut.ConvertToUsdAsync(500m, "USD", new DateTime(2024, 1, 1));

        result.Should().Be(500m);
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task ConvertToUsdAsync_applies_historical_rate()
    {
        var handler = new StubHandler(_ => Json("{\"amount\":1.0,\"base\":\"EUR\",\"date\":\"2024-01-15\",\"rates\":{\"USD\":1.0873}}"));
        var sut = Build(handler);

        var result = await sut.ConvertToUsdAsync(120000m, "EUR", new DateTime(2024, 1, 15));

        result.Should().Be(Math.Round(120000m * 1.0873m, 2));
    }

    [Fact]
    public async Task ConvertToUsdAsync_returns_null_on_http_failure()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var sut = Build(handler);

        var result = await sut.ConvertToUsdAsync(100m, "EUR", new DateTime(2024, 1, 1));

        result.Should().BeNull();
    }

    [Fact]
    public async Task ConvertToUsdAsync_returns_null_on_exception()
    {
        var handler = new StubHandler(_ => throw new HttpRequestException("network down"));
        var sut = Build(handler);

        var result = await sut.ConvertToUsdAsync(100m, "GBP", new DateTime(2024, 1, 1));

        result.Should().BeNull();
    }

    [Fact]
    public async Task ConvertToUsdAsync_caches_rate_and_calls_api_once_for_same_currency_and_date()
    {
        var handler = new StubHandler(_ => Json("{\"amount\":1.0,\"base\":\"GBP\",\"date\":\"2024-03-01\",\"rates\":{\"USD\":1.25}}"));
        var sut = Build(handler);
        var date = new DateTime(2024, 3, 1);

        var first = await sut.ConvertToUsdAsync(100m, "GBP", date);
        var second = await sut.ConvertToUsdAsync(200m, "GBP", date);

        first.Should().Be(125m);
        second.Should().Be(250m);
        handler.CallCount.Should().Be(1); // second lookup served from cache
    }
}
