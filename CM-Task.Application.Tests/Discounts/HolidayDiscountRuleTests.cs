using CM_Task.Application.Abstractions;
using CM_Task.Application.Discounts.Rules;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Discounts;

public sealed class HolidayDiscountRuleTests
{
    private readonly IPublicHolidayService _publicHolidayService = Substitute.For<IPublicHolidayService>();

    private HolidayDiscountRule CreateSut() => new(_publicHolidayService);

    [Fact]
    public async Task Calculate_ReturnsDiscount_WhenHoliday()
    {
        _publicHolidayService
            .IsPublicHolidayAsync(Arg.Any<DateOnly>(), "PL", Arg.Any<CancellationToken>())
            .Returns(true);

        var context = DiscountContextMother.WithLines(
            DiscountContextMother.PolishHoliday,
            CustomerMother.Europe(), (100m, 2), (200m, 1));
        var sut = CreateSut();

        var result = await sut.Calculate(context);

        result.Should().NotBeNull();
        result.Amount.Should().Be(200m * 1 * 0.15m);
        result.Name.Should().Be("Holiday 15%");
    }

    [Fact]
    public async Task Calculate_ReturnsNull_WhenNotHoliday()
    {
        _publicHolidayService
            .IsPublicHolidayAsync(Arg.Any<DateOnly>(), "PL", Arg.Any<CancellationToken>())
            .Returns(false);

        var context = DiscountContextMother.WithLines(
            DiscountContextMother.RegularDay,
            CustomerMother.Europe(),
            (100m, 1));
        var sut = CreateSut();

        var result = await sut.Calculate(context);

        result.Should().BeNull();
    }
}