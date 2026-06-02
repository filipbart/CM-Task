using CM_Task.Application.Discounts.Rules;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using Xunit;

namespace CM_Task.Application.Tests.Discounts;

public sealed class BlackFridayDiscountRuleTests
{
    private readonly BlackFridayDiscountRule _sut = new();

    [Fact]
    public async Task Calculate_ReturnsDiscount_OnBlackFriday()
    {
        var context = DiscountContextMother.WithLines(
            DiscountContextMother.BlackFriday2026,
            CustomerMother.Usa(),
            (100m, 1));

        var result = await _sut.Calculate(context);

        result.Should().NotBeNull();
        result.Amount.Should().Be(100m * 0.25m);
    }

    [Fact]
    public async Task Calculate_ReturnsNull_OnRegularFriday()
    {
        var regularFriday = new DateOnly(2026, 6, 15);
        var context = DiscountContextMother.WithLines(
            regularFriday,
            CustomerMother.Usa(),
            (100m, 1));

        var result = await _sut.Calculate(context);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(2024, 11, 29)] // Black Friday 2024
    [InlineData(2025, 11, 28)] // Black Friday 2025
    [InlineData(2026, 11, 27)] // Black Friday 2026
    public async Task Calculate_ReturnsDiscount_ForMultipleYears(int year, int month, int day)
    {
        var date = new DateOnly(year, month, day);
        var context = DiscountContextMother.WithLines(
            date,
            CustomerMother.Usa(),
            (100m, 1));

        var result = await _sut.Calculate(context);

        result.Should().NotBeNull();
        result.Amount.Should().Be(100m * 0.25m);
    }
}