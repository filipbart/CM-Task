using CM_Task.Application.Abstractions;

namespace CM_Task.Application.Discounts.Rules;

public sealed class VolumeDiscountRule : IDiscountRule
{
    public Task<DiscountResult?> Calculate(DiscountContext ctx, CancellationToken ct = default)
    {
        var totalUnits = ctx.Lines.Sum(l => l.Quantity);
        var percentage = totalUnits switch
        {
            >= 50 => 0.30m,
            >= 10 => 0.20m,
            >= 5 => 0.10m,
            _ => 0m
        };

        var orderTotal = ctx.Lines.Sum(l => l.LineTotal);
        return Task.FromResult(
            percentage == 0m ? null : new DiscountResult($"Volume {percentage:P0}", orderTotal * percentage));
    }
}