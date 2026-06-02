using CM_Task.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CM_Task.Infrastructure.Tests.Services;

public sealed class NagerPublicHolidayServiceTests
{
    private const string IndependenceDayResponse =
        """[{"date":"2025-11-11","localName":"Narodowe Święto Niepodległości","countryCode":"PL"}]""";

    private static NagerPublicHolidayService CreateSut(StubHttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://date.nager.at/") });

    [Fact]
    public async Task IsPublicHolidayAsync_ReturnsTrue_ForKnownHoliday()
    {
        var sut = CreateSut(new StubHttpMessageHandler(IndependenceDayResponse));

        var result = await sut.IsPublicHolidayAsync(new DateOnly(2025, 11, 11), "PL");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPublicHolidayAsync_ReturnsFalse_ForNonHoliday()
    {
        var sut = CreateSut(new StubHttpMessageHandler(IndependenceDayResponse));

        var result = await sut.IsPublicHolidayAsync(new DateOnly(2025, 6, 15), "PL");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetHolidaysForYearAsync_ReturnsEmptySet_WhenApiReturnsEmptyArray()
    {
        var sut = CreateSut(new StubHttpMessageHandler("[]"));

        var result = await sut.GetHolidaysForYearAsync(2025, "PL");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHolidaysForYearAsync_ParsesDates()
    {
        var sut = CreateSut(new StubHttpMessageHandler(IndependenceDayResponse));

        var result = await sut.GetHolidaysForYearAsync(2025, "PL");

        result.Should().ContainSingle()
            .Which.Should().Be(new DateOnly(2025, 11, 11));
    }
}
