using CM_Task.Application.Abstractions;
using CM_Task.Application.Discounts;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Discounts;

public sealed class DiscountEngineTests
{
    private static IDiscountRule MakeRule(decimal? amount)
    {
        var rule = Substitute.For<IDiscountRule>();
        rule.Calculate(Arg.Any<DiscountContext>(), Arg.Any<CancellationToken>())
            .Returns(amount.HasValue ? new DiscountResult("Rule", amount.Value) : null);
        return rule;
    }

    [Fact]
    public async Task GetBestDiscount_ReturnsHighest_WhenMultipleApply()
    {
        var engine = new DiscountEngine([MakeRule(10m), MakeRule(30m), MakeRule(20m)]);

        var result = await engine.GetBestDiscount(DiscountContextMother.Default(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Amount.Should().Be(30m);
    }

    [Fact]
    public async Task GetBestDiscount_ReturnsNull_WhenNoRuleApplies()
    {
        var engine = new DiscountEngine([MakeRule(null), MakeRule(null)]);

        var result = await engine.GetBestDiscount(DiscountContextMother.Default(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBestDiscount_DoesNotCombineDiscounts()
    {
        var engine = new DiscountEngine([MakeRule(10m), MakeRule(30m)]);

        var result = await engine.GetBestDiscount(DiscountContextMother.Default(), CancellationToken.None);

        result!.Amount.Should().Be(30m);
    }
}