using CM_Task.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace CM_Task.Infrastructure.Tests.Services;

public sealed class CachedPublicHolidayServiceTests
{
    private const string IndependenceDayResponse =
        """[{"date":"2025-11-11","localName":"Narodowe Święto Niepodległości","countryCode":"PL"}]""";

    private static (CachedPublicHolidayService sut, StubHttpMessageHandler handler) CreateSut()
    {
        var handler = new StubHttpMessageHandler(IndependenceDayResponse);
        var inner = new NagerPublicHolidayService(
            new HttpClient(handler) { BaseAddress = new Uri("https://date.nager.at/") });
        var cache = new MemoryCache(new MemoryCacheOptions());
        return (new CachedPublicHolidayService(inner, cache), handler);
    }

    [Fact]
    public async Task IsPublicHolidayAsync_ReturnsExpectedResult()
    {
        var (sut, _) = CreateSut();

        var result = await sut.IsPublicHolidayAsync(new DateOnly(2025, 11, 11), "PL");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPublicHolidayAsync_CachesPerYear_AndDoesNotCallApiTwice()
    {
        var (sut, handler) = CreateSut();

        await sut.IsPublicHolidayAsync(new DateOnly(2025, 11, 11), "PL");
        await sut.IsPublicHolidayAsync(new DateOnly(2025, 6, 15), "PL");

        handler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task IsPublicHolidayAsync_CallsApiAgain_ForDifferentYear()
    {
        var (sut, handler) = CreateSut();

        await sut.IsPublicHolidayAsync(new DateOnly(2025, 11, 11), "PL");
        await sut.IsPublicHolidayAsync(new DateOnly(2024, 11, 11), "PL");

        handler.CallCount.Should().Be(2);
    }
}