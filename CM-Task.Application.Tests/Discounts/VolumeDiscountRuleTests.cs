using CM_Task.Application.Discounts.Rules;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using Xunit;

namespace CM_Task.Application.Tests.Discounts;

public sealed class VolumeDiscountRuleTests
{
    private readonly VolumeDiscountRule _sut = new();

    [Theory]
    [InlineData(1, null)] // brak zniżki
    [InlineData(4, null)] // poniżej progu
    [InlineData(5, 0.10)] // dokładnie próg 10%
    [InlineData(9, 0.10)]
    [InlineData(10, 0.20)] // dokładnie próg 20%
    [InlineData(49, 0.20)]
    [InlineData(50, 0.30)] // dokładnie próg 30%
    [InlineData(100, 0.30)]
    public async Task Calculate_ReturnsCorrectDiscount_BasedOnTotalUnits(int totalQty, double? expectedPct)
    {
        var context = DiscountContextMother.WithLines(
            DiscountContextMother.RegularDay,
            CustomerMother.Usa(),
            (100m, totalQty));

        var result = await _sut.Calculate(context);

        if (expectedPct is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().NotBeNull();
            result.Amount.Should().Be(100m * totalQty * (decimal)expectedPct);
        }
    }

    [Fact]
    public async Task Calculate_SumsAllLinesQuantity_NotPerLine()
    {
        var context = DiscountContextMother.WithLines(
            DiscountContextMother.RegularDay,
            CustomerMother.Usa(),
            (100m, 3), (100m, 4));

        var result = await _sut.Calculate(context);

        result.Should().NotBeNull();
        result.Amount.Should().Be(700m * 0.10m);
    }
}